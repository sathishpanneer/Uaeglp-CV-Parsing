using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.MongoModels;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.Utilities;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Services
{
    public class UserRecommendationService : IUserRecommendationService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly AppDbContext _appDbContext;
        private readonly MongoDbContext _mongoDbContext;
        private readonly IMapper _mapper;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IUserIPAddress _userIPAddress;

        public UserRecommendationService(MongoDbContext mongoDbContext, AppDbContext appDbContext, IMapper mapper, IPushNotificationService pushNotificationService, IUserIPAddress userIPAddress)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _pushNotificationService = pushNotificationService;
            _mongoDbContext = mongoDbContext;
            _userIPAddress = userIPAddress;
        }

        public async Task<IUserRecommendationResponse> SendRecommendationAsync(MultipleUserRecommendationModelView view)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {view.ToJsonString()} UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                var firstName = await _appDbContext.Profiles.Where(k => k.Id == view.SenderUserID).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
                var lastName = await _appDbContext.Profiles.Where(k => k.Id == view.SenderUserID).Select(k => k.LastNameEn).FirstOrDefaultAsync();
                var userName = firstName + " " + lastName;

                var notifyText = view.isAskedRecommendation ? userName + " is asked for recommendation" : userName + " has sent to you recommendation";

                List<UserRecommendationModelView> UserRecommendViews = new List<UserRecommendationModelView>();

                foreach (int RecipientUserID in view.RecipientUserID)
                {


                    var data = new UserRecommendation()
                    {
                        SenderUserID = view.SenderUserID,
                        RecipientUserID = RecipientUserID,
                        RecommendationText = view.RecommendationText,
                        isAskedRecommendation = view.isAskedRecommendation,
                        isRead = false,
                        Created = DateTime.Now,
                        CreatedBy = userName,
                        Modified=DateTime.Now,
                        ModifiedBy= userName
                    };

                    await _appDbContext.AddAsync(data);
                    await _appDbContext.SaveChangesAsync();


                    var userRecommend = _mapper.Map<UserRecommendationModelView>(data);



                    //var deviceIds = await _appDbContext.UserDeviceInfos.Where(k => k.UserId == RecipientUserID).Select(k => k.DeviceId).ToListAsync();

                    //if (deviceIds.Count > 0)
                    //{
                    //    foreach (var deviceId in deviceIds)
                    //    {
                    //        logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  Sending recommend push notification to User : {RecipientUserID} and Device ID: {deviceId} ");
                    //        await _pushNotificationService.SendRecommendPushNotificationAsync(userRecommend, notifyText, deviceId);
                    //    }
                    //    logger.Info("Notification sent");
                    //}

                    var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == data.RecipientUserID && x.CategoryID == (int)CategoryType.Messages).FirstOrDefaultAsync();
                    if (customNotificationData?.isEnabled == true || customNotificationData == null)
                    {
                        await AddNotificationAsync(data.RecipientUserID, ActionType.AddNewItem, data.ID, data.isAskedRecommendation ? ParentType.AskRecommendation : ParentType.SendRecommendation, data.SenderUserID, notifyText);
                    }

                    UserRecommendViews.Add(userRecommend);

                }

                return new UserRecommendationResponse(UserRecommendViews);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new UserRecommendationResponse(ex);
            }



        }
        public async Task<IUserRecommendationResponse> AddNotificationAsync(int recipientUserID, ActionType actionId, int recommendId, ParentType parentTypeId, int senderUserId, string notifyText)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {senderUserId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                //var profileId = userId != senderUserId ? senderUserId : userId;
                var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == recipientUserID).FirstOrDefaultAsync() ??
                                                await AddNotificationObjectAsync(recipientUserID);

                var notificationObj = new Notification
                {
                    ID = ObjectId.GenerateNewId(),
                    ActionID = (int)actionId,
                    IsRead = false,
                    ParentID = recommendId.ToString(),
                    ParentTypeID = (int)parentTypeId,
                    SenderID = senderUserId
                };

                notificationGenericObject.Notifications.Add(notificationObj);

                //if (userId != senderUserId)
                //{
                notificationGenericObject.UnseenNotificationCounter += 1;
                var notificationView = _mapper.Map<NotificationView>(notificationObj);
                await FillNotificationUserDetailsAsync(recipientUserID, new List<NotificationView>() { notificationView });

                var deviceIds = await _appDbContext.UserDeviceInfos.Where(k => k.UserId == recipientUserID).Select(k => k.DeviceId).ToListAsync();
                foreach (var deviceId in deviceIds)
                {
                    await _pushNotificationService.SendRecommendPushNotificationAsync(notificationView, notifyText, deviceId);
                }

                logger.Info("Notification sent");
                //}

                await _mongoDbContext.NotificationGenericObjects.ReplaceOneAsync(x => x.UserID == recipientUserID, notificationGenericObject);



                var notificationGenericObjectView = new NotificationGenericObjectView
                {
                    ID = notificationGenericObject.ID.ToString(),
                    UserID = notificationGenericObject.UserID,
                    UnseenNotificationCounter = notificationGenericObject.UnseenNotificationCounter,
                    NotificationsList = _mapper.Map<List<NotificationView>>(notificationGenericObject.Notifications)
                };

                return new UserRecommendationResponse(notificationGenericObjectView);
            }
            catch (Exception e)
            {
                return new UserRecommendationResponse(e);
            }
        }

        //public async Task<bool> PushNotification(NotificationView notification, string notifyText, int userId)
        //{
        //    var deviceIds = await _appDbContext.UserDeviceInfos.Where(k => k.UserId == userId).Select(k => k.DeviceId).ToListAsync();
        //    foreach (var deviceId in deviceIds)
        //    {
        //        await _pushNotificationService.SendRecommendPushNotificationAsync(notification, notifyText, deviceId);
        //    }
        //    return true;
        //}
        private async Task FillNotificationUserDetailsAsync(int userId, List<NotificationView> notificationsList)
        {
            foreach (var notification in notificationsList)
            {
                var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == notification.SenderID);
                notification.UserNameEn = user?.NameEn;
                notification.UserNameAr = user?.NameAr;
                notification.UserImageFileId = user?.OriginalImageFileId ?? 0;
                notification.RedirectUrlPath = notification.ParentTypeID == ParentType.User
                    ? $"/api/Profile/get-public-profile/{userId}/{notification.SenderID}"
                    : notification.RedirectUrlPath;
            }
        }
        private async Task<NotificationGenericObject> AddNotificationObjectAsync(int userId)
        {
            try
            {
                var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync();

                if (notificationGenericObject != null) return notificationGenericObject;

                notificationGenericObject = new NotificationGenericObject
                {
                    ID = ObjectId.GenerateNewId(),
                    UserID = userId,
                    UnseenNotificationCounter = 0,
                    Notifications = new List<Notification>()
                };

                await _mongoDbContext.NotificationGenericObjects.InsertOneAsync(notificationGenericObject);
                return notificationGenericObject;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<IUserRecommendationResponse> AcceptDeclineRecommendationAsync(int recommendId,int recipientUserId, bool isAccepted, bool isDeclined)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {recipientUserId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                var data = await _appDbContext.UserRecommendations.Where(x => x.ID == recommendId && x.RecipientUserID == recipientUserId).FirstOrDefaultAsync();

                if (data != null)
                {
                    data.isAccepted = isAccepted;
                    data.isDeclined = isDeclined;
                    await _appDbContext.SaveChangesAsync();
                }
                var userRecommend = _mapper.Map<UserRecommendationModelView>(data);
                return new UserRecommendationResponse(userRecommend);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new UserRecommendationResponse(ex);
            }
        }


        public async Task<IUserRecommendationResponse> ReceiveRecommendationAsync(int recipientUserId, int recommendID)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() } UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                var data = await _appDbContext.UserRecommendations.Where(x => x.RecipientUserID == recipientUserId && x.ID == recommendID).FirstOrDefaultAsync();

                //var recommendDetails = _mapper.Map<UserRecommendationModelView>(data);
                var userRecommend = _mapper.Map<UserRecommendationModelView>(data);
                return new UserRecommendationResponse(userRecommend); 
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new UserRecommendationResponse(ex);
            }
        }
    

        public async Task<IUserRecommendationResponse> ReceiveAllRecommendationAsync(int recipientUserId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() } UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                var data = await _appDbContext.UserRecommendations.Where(x => x.RecipientUserID == recipientUserId && x.isAccepted != true).ToListAsync();

                var recommendDetails = _mapper.Map<List<UserRecommendView>>(data);



                foreach(var item in recommendDetails)
                {
                    var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == item.SenderUserID);
                    var workExperience = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title)
                        .Where(k => k.ProfileId == item.SenderUserID).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();
                    var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == item.SenderUserID);
                    item.UserInfo = new PublicProfileView()
                    {
                        Id = profile.Id,
                        FirstNameAr = profile.FirstNameAr ?? "",
                        FirstNameEn = profile.FirstNameEn ?? "",
                        LastNameAr = profile.LastNameAr ?? "",
                        LastNameEn = profile.LastNameEn ?? "",
                        SecondNameAr = profile.SecondNameAr ?? "",
                        SecondNameEn = profile.SecondNameEn ?? "",
                        ThirdNameAr = profile.ThirdNameAr ?? "",
                        ThirdNameEn = profile.ThirdNameEn ?? "",
                        Designation = workExperience?.Title?.TitleEn ?? "",
                        DesignationAr = workExperience?.Title?.TitleAr ?? "",
                        UserImageFileId = user?.OriginalImageFileId ?? 0,
                        About = ""
                    };
                }
                var recommendInfo = new UserRecommendationDetails()
                {
                    RecommendationInfo = recommendDetails
                };


                return new UserRecommendationResponse(recommendInfo);
               
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new UserRecommendationResponse(ex);
            }
        }

        public async Task<IUserRecommendationResponse> ReceiveAcceptedAllRecommendationAsync(int recipientUserId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() } UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                var data = await _appDbContext.UserRecommendations.Where(x => x.RecipientUserID == recipientUserId && x.isAccepted == true).ToListAsync();

                var recommendDetails = _mapper.Map<List<UserRecommendView>>(data);


                foreach (var item in recommendDetails)
                {
                    var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == item.SenderUserID);
                    var workExperience = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title)
                        .Where(k => k.ProfileId == item.SenderUserID).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();
                    var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == item.SenderUserID);
                    item.UserInfo =  new PublicProfileView()
                    {
                        Id = profile.Id,
                        FirstNameAr = profile.FirstNameAr?? "",
                        FirstNameEn = profile.FirstNameEn?? "",
                        LastNameAr = profile.LastNameAr ?? "",
                        LastNameEn = profile.LastNameEn ?? "",
                        SecondNameAr = profile.SecondNameAr ?? "",
                        SecondNameEn = profile.SecondNameEn ?? "",
                        ThirdNameAr = profile.ThirdNameAr ?? "",
                        ThirdNameEn = profile.ThirdNameEn ?? "",
                        Designation = workExperience?.Title?.TitleEn ?? "",
                        DesignationAr = workExperience?.Title?.TitleAr ?? "",
                        UserImageFileId = user?.OriginalImageFileId ?? 0,
                        About = ""
                    };
                }


                var recommendInfo = new UserRecommendationDetails()
                {
                    RecommendationInfo = recommendDetails.Select(x => new UserRecommendView
                    {
                        ID = x.ID,
                        SenderUserID = x.SenderUserID,
                        RecipientUserID = x.RecipientUserID,
                        RecommendationText = x.RecommendationText,
                        isAccepted = x.isAccepted,
                        isAskedRecommendation = x.isAskedRecommendation,
                        UserInfo = x.UserInfo
                    }).ToList()
                };

                return new UserRecommendationResponse(recommendInfo);
               
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new UserRecommendationResponse(ex);
            }
        }

        public async Task<IUserRecommendationResponse> SetReadRecommendationAsync(int recommendId, int recipientUserId, bool isread)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {recipientUserId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                var data = await _appDbContext.UserRecommendations.Where(x => x.ID == recommendId && x.RecipientUserID == recipientUserId).FirstOrDefaultAsync();

                if (data != null)
                {
                    data.isRead = isread;
                    
                    await _appDbContext.SaveChangesAsync();
                }
                var recommendDetails = _mapper.Map<UserRecommendationModelView>(data);
                return new UserRecommendationResponse(recommendDetails);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new UserRecommendationResponse(ex);
            }
        }

        public async Task<IUserRecommendationResponse> DeleteRecommendationAsync(int recommendID)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {recommendID} UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                var data = await _appDbContext.UserRecommendations.Where(x => x.ID == recommendID).FirstOrDefaultAsync();

                if (data != null)
                {

                    _appDbContext.Remove(data);
                    await _appDbContext.SaveChangesAsync();
                }
                var recommendDetails = _mapper.Map<UserRecommendationModelView>(data);
                return new UserRecommendationResponse(true);  
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new UserRecommendationResponse(ex);
            }
        }
    }
}
