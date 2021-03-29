using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contracts;
using Uaeglp.Repositories;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Uaeglp.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NLog;
using Uaeglp.Utilities;
using Uaeglp.MongoModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels;
using AutoMapper;
using Uaeglp.Services.Communication;
using Uaeglp.Contract.Communication;

namespace Uaeglp.Services
{
    public class WebPortalNotificationService : IWebPortalNotificationService
    {
        private readonly IEmailService _emailService;
        private readonly AppDbContext _appDbContext;
        private readonly MongoDbContext _mongoDbContext;
        private readonly IPushNotificationService _pushNotificationService;
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IMapper _mapper;

        public WebPortalNotificationService(IEmailService emailService, AppDbContext appDbContext, IPushNotificationService pushNotificationService, MongoDbContext mongoDbContext, IMapper mapper)
        {
            _emailService = emailService;
            _appDbContext = appDbContext;
            _pushNotificationService = pushNotificationService;
            _mongoDbContext = mongoDbContext;
            _mapper = mapper;
        }

        //public async Task<bool> CustomNotitification()
        //{
        //    await PostLikeNotification();
        //    await PostCommentNotification();
        //    await PostShareNotification();
        //    await ActivityAndChallengeNotification();
        //    await ProgrammeNotification();
        //    await BatchNotification();
        //    await UserFollowingNotification();
        //    await NetworkGroupNotification();
        //    await AssessmentAssignedNotification();
        //    await AssessmentMemberNotification();
        //    await MeetupNotification();
        //    return true;
        //}

        public async Task<bool> PostLikeNotification()
        {
            //var postLike = await _mongoDbContext.Posts.Find(_ => true).Sort(new BsonDocument("LikesTimestamp.Datetime", -1)).Limit(1).FirstOrDefaultAsync();
            
            //var userIdLike = postLike.LikesTimestamp.LastOrDefault().UserID;
            //var postIdLike = postLike.ID.ToString();
            //var parentTypeIdLike = ParentType.Post;
            //var actionIdLike = ActionType.Like;
            //var postTypeIdLike = postLike.TypeID;

            //var notify = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == postLike.UserID).FirstOrDefaultAsync();

            //var Count = 0;
            var mainobj = await _mongoDbContext.NotificationGenericObjects.Find(_ => true).ToListAsync();


            foreach (var items in mainobj)
            {
                var noteObj = items.Notifications.Where(o => o.IsPushed == false && o.ActionID == 1);

                foreach (var item in noteObj)
                {
                    var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == items.UserID && x.CategoryID == (int)CategoryType.SocialNetworking).FirstOrDefaultAsync();
                    if (customNotificationData?.isEnabled == true || customNotificationData == null)
                    {
                        await AddPushNotificationAsync(items.UserID, item);
                    }
                }
            }
            //if (Count == 0)
            //{
            //    var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == postLike.UserID && x.CategoryID == (int)CategoryType.SocialNetworking).FirstOrDefaultAsync();
            //    if (customNotificationData?.isEnabled == true || customNotificationData == null)
            //    {
            //        await AddNotificationAsync(postLike.UserID, postLike.TypeID, ActionType.Like, postLike.ID.ToString(), ParentType.Post, userIdLike);
            //    }
            //}
            return true;
        }

        public async Task<bool> PostCommentNotification()
        {

            var mainobj = await _mongoDbContext.NotificationGenericObjects.Find(_ => true).ToListAsync();


            foreach (var items in mainobj)
            {
                var noteObj = items.Notifications.Where(o => o.IsPushed == false && o.ActionID == 2);

                foreach (var item in noteObj)
                {
                    var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == items.UserID && x.CategoryID == (int)CategoryType.SocialNetworking).FirstOrDefaultAsync();
                    if (customNotificationData?.isEnabled == true || customNotificationData == null)
                    {
                        await AddPushNotificationAsync(items.UserID, item);
                    }
                }
            }
            //var postComment = await _mongoDbContext.Posts.Find(_ => true).Sort(new BsonDocument("Comments.Created", -1)).Limit(1).FirstOrDefaultAsync();

            //var userIdComment = postComment.Comments.LastOrDefault().UserID;
            //var postIdComment = postComment.ID.ToString();
            //var parentTypeIdComment = ParentType.Post;
            //var actionIdComment = ActionType.Comment;
            //var postTypeIdComment = postComment.TypeID;

            //var notify = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == postComment.UserID).FirstOrDefaultAsync();

            //var Count = 0;
            //foreach (var item in notify.Notifications)
            //{
            //    if (item.SenderID == userIdComment && item.ParentID == postIdComment && item.ParentTypeID == (int)parentTypeIdComment && item.ActionID == (int)actionIdComment) //&& item.PostTypeID == postTypeIdComment
            //    {
            //        Count++; //1 // 0
            //    }
            //}
            //if (Count == 0)
            //{
            //    var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == postComment.UserID && x.CategoryID == (int)CategoryType.SocialNetworking).FirstOrDefaultAsync();
            //    if (customNotificationData?.isEnabled == true || customNotificationData == null)
            //    {
            //        await AddNotificationAsync(postComment.UserID, postComment.TypeID, ActionType.Comment, postComment.ID.ToString(), ParentType.Post, userIdComment);
            //    }
            //}
            return true;
        }

        public async Task<bool> PostShareNotification()
        {
            var mainobj = await _mongoDbContext.NotificationGenericObjects.Find(_ => true).ToListAsync();


            foreach (var items in mainobj)
            {
                var noteObj = items.Notifications.Where(o => o.IsPushed == false && o.ActionID == 3);

                foreach (var item in noteObj)
                {
                    var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == items.UserID && x.CategoryID == (int)CategoryType.SocialNetworking).FirstOrDefaultAsync();
                    if (customNotificationData?.isEnabled == true || customNotificationData == null)
                    {
                        await AddPushNotificationAsync(items.UserID, item);
                    }
                }
            }
            //var postShare = await _mongoDbContext.Posts.Find(_ => true).Sort(new BsonDocument("SharesTimestamp.Datetime", -1)).Limit(1).FirstOrDefaultAsync();

            //var userIdShare = postShare.SharesTimestamp.LastOrDefault().UserID;
            //var postIdShare = postShare.ID.ToString();
            //var parentTypeIdShare = ParentType.Post;
            //var actionIdShare = ActionType.Share;
            //var postTypeIdShare = postShare.TypeID;

            //var notify = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == postShare.UserID).FirstOrDefaultAsync();

            //var Count = 0;
            //foreach (var item in notify.Notifications)
            //{
            //    if (item.SenderID == userIdShare && item.ParentID == postIdShare && item.ParentTypeID == (int)parentTypeIdShare && item.ActionID == (int)actionIdShare) //&& item.PostTypeID == postTypeIdShare
            //    {
            //        Count++; //1 // 0
            //    }
            //}
            //if (Count == 0)
            //{
            //    var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == postShare.UserID && x.CategoryID == (int)CategoryType.SocialNetworking).FirstOrDefaultAsync();
            //    if (customNotificationData?.isEnabled == true || customNotificationData == null)
            //    {
            //        await AddNotificationAsync(postShare.UserID, postShare.TypeID, ActionType.Share, postShare.ID.ToString(), ParentType.Post, userIdShare);
            //    }
            //}
            return true;
        }

        public async Task<bool> UserFollowingNotification()
        {
            var mainobj = await _mongoDbContext.NotificationGenericObjects.Find(_ => true).ToListAsync();


            foreach (var items in mainobj)
            {
                var noteObj = items.Notifications.Where(o => o.IsPushed == false && o.ActionID == 4);

                foreach (var item in noteObj)
                {
                    var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == items.UserID && x.CategoryID == (int)CategoryType.SocialNetworking).FirstOrDefaultAsync();
                    if (customNotificationData?.isEnabled == true || customNotificationData == null)
                    {
                        await AddPushNotificationAsync(items.UserID, item);
                    }
                }
            }

            return true;
        }

        public async Task<bool> EventNotification()
        {
            try
            {
                var mainobj = await _mongoDbContext.NotificationGenericObjects.Find(_ => true).ToListAsync();


                foreach (var items in mainobj)
                {
                    var noteObj = items.Notifications.Where(o => o.IsPushed == false && o.ParentTypeID == 8 && o.ActionID == 6);

                    foreach (var item in noteObj)
                    {
                        var events = await _appDbContext.Events.Where(x => x.Id == Convert.ToInt32(item.ParentID)).FirstOrDefaultAsync();
                        if(events != null)
                        {
                            
                            var notifyText = "A new event" + " " + events.TextEn + " " + "has been added";
                                await AddOtherModulePushNotificationAsync(items.UserID, item, notifyText);
                        }
                        

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }

        }
        public async Task<bool> ActivityAndChallengeNotification()
        {
            try
            {

            var mainobj = await _mongoDbContext.NotificationGenericObjects.Find(_ => true).ToListAsync();


            foreach (var items in mainobj)
            {
                var noteObj = items.Notifications.Where(o => o.IsPushed == false && o.ParentTypeID == 5 && o.ActionID == 6);

                foreach (var item in noteObj)
                {
                    var activity = await _appDbContext.Initiatives.Where(x => x.Id == Convert.ToInt32(item.ParentID)).FirstOrDefaultAsync();
                    if(activity != null)
                        {
                            var notifyText = "A new Challenge" + " " + activity.TitleEn + " " + "has been added";

                                var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == items.UserID && x.CategoryID == (int)CategoryType.Activities).FirstOrDefaultAsync();
                                if (customNotificationData?.isEnabled == true || customNotificationData == null)
                                {
                                    await AddOtherModulePushNotificationAsync(items.UserID, item, notifyText);
                                }
                        }
                    
                    

                }
            }
            //var activity = await _appDbContext.Initiatives.OrderByDescending(x => x.Created).FirstOrDefaultAsync();
            //var userId = await _appDbContext.UserInfos.Where(x => x.Email == activity.CreatedBy).Select(x => x.UserId).FirstOrDefaultAsync();

            //var userIdActivity = userId;
            //var postIdActivity = activity.Id.ToString();
            //var parentTypeIdActivity = ParentType.Challenge;
            //var actionIdActivity = ActionType.AddNewItem;
            ////var postTypeIdShare = postShare.TypeID;
            //var mongoUser = (await _mongoDbContext.Users.Find(k => k.Id == userId).FirstOrDefaultAsync()) ??
            //            new MongoModels.User() { };

            //var userIds = new List<int>();
            //userIds.AddRange(mongoUser.FollowersIDs);

            //var notifyText = "A new activity has been added";
            //foreach (var follwingItem in userIds)
            //{
            //    var notify = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == follwingItem).FirstOrDefaultAsync();

            //    var Count = 0;
            //    foreach (var item in notify.Notifications)
            //    {
            //        if (item.SenderID == userIdActivity && item.ParentID == postIdActivity && item.ParentTypeID == (int)parentTypeIdActivity && item.ActionID == (int)actionIdActivity) //&& item.PostTypeID == 0
            //        {
            //            Count++; //1 // 0
            //        }
            //    }
            //    if (Count == 0)
            //    {
            //        var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == follwingItem && x.CategoryID == (int)CategoryType.Activities).FirstOrDefaultAsync();
            //        if (customNotificationData?.isEnabled == true || customNotificationData == null)
            //        {
            //            await AddNotificationActivityAsync(follwingItem, ActionType.AddNewItem, activity.Id.ToString(), ParentType.Challenge, userId, notifyText, "");
            //        }
            //    }
            //}


            return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        public async Task<bool> EngagementActiity()
        {
            try
            {

                var mainobj = await _mongoDbContext.NotificationGenericObjects.Find(_ => true).ToListAsync();


                foreach (var items in mainobj)
                {
                    var noteObj = items.Notifications.Where(o => o.IsPushed == false && o.ParentTypeID == 6 && o.ActionID == 6);

                    foreach (var item in noteObj)
                    {
                        var activity = await _appDbContext.Initiatives.Where(x => x.Id == Convert.ToInt32(item.ParentID)).FirstOrDefaultAsync();
                        if (activity != null)
                        {
                            var notifyText = "A new new Engagement Activity" + " " + activity.TitleEn + " " + "has been added";

                            var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == items.UserID && x.CategoryID == (int)CategoryType.Activities).FirstOrDefaultAsync();
                            if (customNotificationData?.isEnabled == true || customNotificationData == null)
                            {
                                await AddOtherModulePushNotificationAsync(items.UserID, item, notifyText);
                            }
                        }



                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        public async Task<bool> MessagingNotification()
        {
            var room = await _mongoDbContext.Rooms.Find(_ => true).Sort(new BsonDocument("LastModifiedOn", -1)).Limit(1).FirstOrDefaultAsync();
            
            if(room.Messages.Count > 0)
            {
                var senderUserId = room.Messages.LastOrDefault().OwnerID;
                //var userIdActivity = userId;
                var messageId = room.Messages.LastOrDefault().ID.ToString();
                var parentTypeId = ParentType.Messaging;
                var actionId = ActionType.AddNewItem;

                var userIds = new List<int>();
                foreach (var item in room.MembersIDs)
                {
                    if (item != senderUserId)
                    {
                        userIds.Add(item);
                    }
                }
                var firstName = await _appDbContext.Profiles.Where(k => k.Id == senderUserId).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
                var lastName = await _appDbContext.Profiles.Where(k => k.Id == senderUserId).Select(k => k.LastNameEn).FirstOrDefaultAsync();
                var userName = firstName + " " + lastName;
                var notifyText = userName + " " + "has sent a message";
                foreach (var recipientUserID in userIds)
                {
                    var notify = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == recipientUserID).FirstOrDefaultAsync();

                    var Count = 0;
                    foreach (var item in notify.Notifications)
                    {
                        if (item.SenderID == senderUserId && item.ParentID == messageId && item.ParentTypeID == (int)parentTypeId && item.ActionID == (int)actionId) //&& item.PostTypeID == 0
                        {
                            Count++; //1 // 0
                        }
                    }
                    if (Count == 0)
                    {
                        var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == recipientUserID && x.CategoryID == (int)CategoryType.Messages).FirstOrDefaultAsync();
                        if (customNotificationData?.isEnabled == true || customNotificationData == null)
                        {
                            await AddNotificationActivityAsync(recipientUserID, ActionType.AddNewItem, messageId, ParentType.Messaging, senderUserId, notifyText, room.ID.ToString());
                        }
                    }
                }
            }

            return true;
        }

        public async Task<bool> ProgrammeNotification()
        {
            var program = await _appDbContext.Programmes.OrderByDescending(x => x.Created).FirstOrDefaultAsync();
            var userId = await _appDbContext.UserInfos.Where(x => x.Email == program.CreatedBy).Select(x => x.UserId).FirstOrDefaultAsync();

            var userIdProgram = userId;
            var postIdProgram = program.Id.ToString();
            var parentTypeIdProgram = ParentType.Programme;
            var actionIdProgram = ActionType.AddNewItem;
            //var postTypeIdShare = postShare.TypeID;
            var mongoUser = (await _mongoDbContext.Users.Find(k => k.Id == userId).FirstOrDefaultAsync()) ??
                        new MongoModels.User() { };

            var userIds = new List<int>();
            userIds.AddRange(mongoUser.FollowersIDs);

            var notifyText = "A new program has been launched or date has been extended";
            foreach (var follwingItem in userIds)
            {
                var notify = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == follwingItem).FirstOrDefaultAsync();

                var Count = 0;
                foreach (var item in notify.Notifications)
                {
                    if (item.SenderID == userIdProgram && item.ParentID == postIdProgram && item.ParentTypeID == (int)parentTypeIdProgram && item.ActionID == (int)actionIdProgram) //&& item.PostTypeID == 0
                    {
                        Count++; //1 // 0
                    }
                }
                if (Count == 0)
                {
                    var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == follwingItem && x.CategoryID == (int)CategoryType.Programe).FirstOrDefaultAsync();
                    if (customNotificationData?.isEnabled == true || customNotificationData == null)
                    {
                        await AddNotificationActivityAsync(follwingItem, ActionType.AddNewItem, program.Id.ToString(), ParentType.Programme, userId, notifyText, "");
                    }
                }
            }


            return true;
        }

        public async Task<bool> BatchNotification()
        {
            try
            {

            
            var mainobj = await _mongoDbContext.NotificationGenericObjects.Find(_ => true).ToListAsync();

            foreach (var items in mainobj)
            {
                var noteObj = items.Notifications.Where(o => o.IsPushed == false && o.ParentTypeID == 7 && o.ActionID == 6);
    
                foreach (var item in noteObj)
                {
                    var batch = await _appDbContext.Batches.Where(x => x.Id == Convert.ToInt32(item.ParentID)).FirstOrDefaultAsync();
                    if(batch != null)
                        {
                            var titleEn = await _appDbContext.Programmes.Where(x => x.Id == batch.ProgrammeId).Select(x => x.TitleEn).FirstOrDefaultAsync();
                            var notifyText = "A new batch" + " " + titleEn +  " " + "has been added";
                                var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == items.UserID && x.CategoryID == (int)CategoryType.Programe).FirstOrDefaultAsync();
                                if (customNotificationData?.isEnabled == true || customNotificationData == null)
                                {
                                    await AddOtherModulePushNotificationAsync(items.UserID, item, notifyText);
                                }
                        }
                }
            }
            //var batch = await _appDbContext.Batches.OrderByDescending(x => x.Created).FirstOrDefaultAsync();
            //var userId = await _appDbContext.UserInfos.Where(x => x.Email == batch.CreatedBy).Select(x => x.UserId).FirstOrDefaultAsync();

            //var userIdBatch = userId;
            //var postIdBatch = batch.Id.ToString();
            //var parentTypeIdBatch = ParentType.Batch;
            //var actionIdBatch = ActionType.AddNewItem;
            ////var postTypeIdShare = postShare.TypeID;
            //var mongoUser = (await _mongoDbContext.Users.Find(k => k.Id == userId).FirstOrDefaultAsync()) ??
            //            new MongoModels.User() { };

            //var userIds = new List<int>();
            //userIds.AddRange(mongoUser.FollowersIDs);

            //var notifyText = "A new batch has been added";
            //foreach (var follwingItem in userIds)
            //{
            //    var notify = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == follwingItem).FirstOrDefaultAsync();

            //    var Count = 0;
            //    foreach (var item in notify.Notifications)
            //    {
            //        if (item.SenderID == userIdBatch && item.ParentID == postIdBatch && item.ParentTypeID == (int)parentTypeIdBatch && item.ActionID == (int)actionIdBatch) //&& item.PostTypeID == 0
            //        {
            //            Count++; //1 // 0
            //        }
            //    }
            //    if (Count == 0)
            //    {
            //        var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == follwingItem && x.CategoryID == (int)CategoryType.Programe).FirstOrDefaultAsync();
            //        if (customNotificationData?.isEnabled == true || customNotificationData == null)
            //        {
            //            await AddNotificationActivityAsync(follwingItem, ActionType.AddNewItem, batch.Id.ToString(), ParentType.Batch, userId, notifyText, "");
            //        }
            //    }
            //}


            return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        public async Task<bool> AssessmentAssignedNotification()
        {
            try { 
            var mainobj = await _mongoDbContext.NotificationGenericObjects.Find(_ => true).ToListAsync();


            foreach (var items in mainobj)
            {
                var noteObj = items.Notifications.Where(o => o.IsPushed == false && o.ParentTypeID == 12 && o.ActionID == 6);

                foreach (var item in noteObj)
                {
                    var assessment = await _appDbContext.AssessmentTools.Where(x => x.Id == Convert.ToInt32(item.ParentID)).FirstOrDefaultAsync();
                    if(assessment != null)
                        {
                            var notifyText = "You have been assigned" + " " + assessment.NameEn + " " + "new assessments";
                                var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == items.UserID && x.CategoryID == (int)CategoryType.Assessments).FirstOrDefaultAsync();
                                if (customNotificationData?.isEnabled == true || customNotificationData == null)
                                {
                                    await AddOtherModulePushNotificationAsync(items.UserID, item, notifyText);
                                }
                        }

                    


                }
            }
            //var assignment = await _appDbContext.AssessmentTools.OrderByDescending(x => x.Created).FirstOrDefaultAsync();
            //var userId = await _appDbContext.UserInfos.Where(x => x.Email == assignment.CreatedBy).Select(x => x.UserId).FirstOrDefaultAsync();

            //var userIdAssessment = userId;
            //var postIdAssessment = assignment.Id.ToString();
            //var parentTypeIdAssessment = ParentType.AssignedAssessment;
            //var actionIdAssessment = ActionType.AddNewItem;
            ////var postTypeIdShare = postShare.TypeID;
            //var mongoUser = (await _mongoDbContext.Users.Find(k => k.Id == userId).FirstOrDefaultAsync()) ??
            //            new MongoModels.User() { };

            //var userIds = new List<int>();
            //userIds.AddRange(mongoUser.FollowersIDs);

            //var notifyText = "You have been assigned" + " " + assignment.NameEn + " " + "new assessments";
            //foreach (var follwingItem in userIds)
            //{
            //    var notify = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == follwingItem).FirstOrDefaultAsync();

            //    var Count = 0;
            //    foreach (var item in notify.Notifications)
            //    {
            //        if (item.SenderID == userIdAssessment && item.ParentID == postIdAssessment && item.ParentTypeID == (int)parentTypeIdAssessment && item.ActionID == (int)actionIdAssessment)//&& item.PostTypeID == 0
            //        {
            //            Count++; //1 // 0
            //        }
            //    }
            //    if (Count == 0)
            //    {
            //        var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == follwingItem && x.CategoryID == (int)CategoryType.Assessments).FirstOrDefaultAsync();
            //        if (customNotificationData?.isEnabled == true || customNotificationData == null)
            //        {
            //            await AddNotificationActivityAsync(follwingItem, ActionType.AddNewItem, assignment.Id.ToString(), ParentType.AssignedAssessment, userId, notifyText, "");
            //        }
            //    }
            //}


            return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        public async Task<bool> NetworkGroupNotification()
        {
            var mainobj = await _mongoDbContext.NotificationGenericObjects.Find(_ => true).ToListAsync();


            foreach (var items in mainobj)
            {
                var noteObj = items.Notifications.Where(o => o.IsPushed == false && o.ParentTypeID == 3 && o.ActionID == 6);

                foreach (var item in noteObj)
                {
                    var networkGroup = await _appDbContext.NetworkGroups.Where(x => x.Id == Convert.ToInt32(item.ParentID)).FirstOrDefaultAsync();
                    if (networkGroup != null)
                    {
                        var notifyText = "You have been added to network group" + " " + networkGroup.NameEn;

                            await AddOtherModulePushNotificationAsync(items.UserID, item, notifyText);
                        
                    }

                }
            }







            //var networkProfiles = await _appDbContext.NetworkGroupProfiles.ToListAsync();
            //var networkProfile = networkProfiles.Last();
            //var networkGroup = await _appDbContext.NetworkGroups.Where(x => x.Id == networkProfile.NetworkGroupId).FirstOrDefaultAsync();


            //var userId = await _appDbContext.UserInfos.Where(x => x.Email == networkGroup.CreatedBy).Select(x => x.UserId).FirstOrDefaultAsync();

            //var userIdGroup = userId;
            //var postIdGroup = networkProfile.NetworkGroupId.ToString();
            //var parentTypeIdGroup = ParentType.Group;
            //var actionIdGroup = ActionType.AddNewItem;
            ////var postTypeIdShare = postShare.TypeID;
            

            //var notifyText = "You have been added to network group" + " " + networkGroup.NameEn;
            ////foreach (var follwingItem in userIds)
            ////{
            //    var notify = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == networkProfile.ProfileId).FirstOrDefaultAsync();

            //    var Count = 0;
            //    foreach (var item in notify.Notifications)
            //    {
            //        if (item.SenderID == userIdGroup && item.ParentID == postIdGroup && item.ParentTypeID == (int)parentTypeIdGroup && item.ActionID == (int)actionIdGroup) //&& item.PostTypeID == 0
            //    {
            //            Count++; //1 // 0
            //        }
            //    }
            //    if (Count == 0)
            //    {
            //        await AddNotificationActivityAsync(networkProfile.ProfileId, ActionType.AddNewItem, networkProfile.NetworkGroupId.ToString(), ParentType.Group, userId, notifyText, "");
            //    }
            //}


            return true;
        }

        public async Task<bool> AssessmentMemberNotification()
        {
            try { 
            var mainobj = await _mongoDbContext.NotificationGenericObjects.Find(_ => true).ToListAsync();


            foreach (var items in mainobj)
            {
                var noteObj = items.Notifications.Where(o => o.IsPushed == false && o.ParentTypeID == 11 && o.ActionID == 6);

                foreach (var item in noteObj)
                {
                    var assesmentGroupMember = await _appDbContext.AssessmentGroupMembers.Where(x => x.Id == Convert.ToInt32(item.ParentID)).FirstOrDefaultAsync();
                        if(assesmentGroupMember != null)
                        {
                            var groupName = await _appDbContext.AssessmentGroups.Where(x => x.Id == assesmentGroupMember.AssessmentGroupId).Select(x => x.NameEn).FirstOrDefaultAsync();
                            var notifyText = "You have been added to assessment group" + " " + groupName;

                            var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == assesmentGroupMember.ProfileId && x.CategoryID == (int)CategoryType.Assessments).FirstOrDefaultAsync();
                            if (customNotificationData?.isEnabled == true || customNotificationData == null)
                            {
                                await AddOtherModulePushNotificationAsync(assesmentGroupMember.ProfileId, item, notifyText);
                            }
                            var email = await _appDbContext.UserInfos.Where(k => k.UserId == assesmentGroupMember.ProfileId).Select(k => k.Email).FirstOrDefaultAsync();
                            var firstName = await _appDbContext.Profiles.Where(k => k.Id == assesmentGroupMember.ProfileId).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
                            var lastName = await _appDbContext.Profiles.Where(k => k.Id == assesmentGroupMember.ProfileId).Select(k => k.LastNameEn).FirstOrDefaultAsync();
                            var userName = firstName + " " + lastName;

                            await _emailService.SendAssessmentReminderAsync(email, groupName, userName);
                        }
                    
                }
            }
            //var member = await _appDbContext.AssessmentGroupMembers.OrderByDescending(x => x.Created).FirstOrDefaultAsync();
            //var groupName = await _appDbContext.AssessmentGroups.Where(x => x.Id == member.AssessmentGroupId).Select(x => x.NameEn).FirstOrDefaultAsync();


            //var userId = await _appDbContext.UserInfos.Where(x => x.Email == member.CreatedBy).Select(x => x.UserId).FirstOrDefaultAsync();

            //var userIdMember = userId;
            //var postIdMember = member.Id.ToString();
            //var parentTypeIdMember = ParentType.AssessmentGroup;
            //var actionIdMember = ActionType.AddNewItem;
            ////var postTypeIdShare = postShare.TypeID;


            //var notifyText = "You have been added to assessment group" + " " + groupName;
            //var notify = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == member.ProfileId).FirstOrDefaultAsync();

            //var Count = 0;
            //foreach (var item in notify.Notifications)
            //{
            //    if (item.SenderID == userIdMember && item.ParentID == postIdMember && item.ParentTypeID == (int)parentTypeIdMember && item.ActionID == (int)actionIdMember) //&& item.PostTypeID == 0
            //    {
            //        Count++; //1 // 0
            //    }
            //}
            //if (Count == 0)
            //{
            //    var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == member.ProfileId && x.CategoryID == (int)CategoryType.Assessments).FirstOrDefaultAsync();
            //    if (customNotificationData?.isEnabled == true || customNotificationData == null)
            //    {
            //        await AddNotificationActivityAsync(member.ProfileId, ActionType.AddNewItem, member.Id.ToString(), ParentType.AssessmentGroup, userId, notifyText, "");
            //    }

            //    var email = await _appDbContext.UserInfos.Where(k => k.UserId == member.ProfileId).Select(k => k.Email).FirstOrDefaultAsync();
            //    var firstName = await _appDbContext.Profiles.Where(k => k.Id == member.ProfileId).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
            //    var lastName = await _appDbContext.Profiles.Where(k => k.Id == member.ProfileId).Select(k => k.LastNameEn).FirstOrDefaultAsync();
            //    var userName = firstName + " " + lastName;

            //    await _emailService.SendAssessmentReminderAsync(email, groupName, userName);
            //}
            ////}


            return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        public async Task<bool> AssessmentCoordinatorNotification()
        {
            var member = await _appDbContext.AssessmentGroupMembers.OrderByDescending(x => x.Created).FirstOrDefaultAsync();
            var groupName = await _appDbContext.AssessmentGroups.Where(x => x.Id == member.AssessmentGroupId).Select(x => x.NameEn).FirstOrDefaultAsync();


            var userId = await _appDbContext.UserInfos.Where(x => x.Email == member.CreatedBy).Select(x => x.UserId).FirstOrDefaultAsync();

            var userIdMember = userId;
            var postIdMember = member.Id.ToString();
            var parentTypeIdMember = ParentType.AssessmentGroup;
            var actionIdMember = ActionType.AddNewItem;
            //var postTypeIdShare = postShare.TypeID;


            var notifyText = "You have been added to assessment group" + " " + groupName;
            //foreach (var follwingItem in userIds)
            //{
            var notify = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == member.ProfileId).FirstOrDefaultAsync();

            var Count = 0;
            foreach (var item in notify.Notifications)
            {
                if (item.SenderID == userIdMember && item.ParentID == postIdMember && item.ParentTypeID == (int)parentTypeIdMember && item.ActionID == (int)actionIdMember) //&& item.PostTypeID == 0
                {
                    Count++; //1 // 0
                }
            }
            if (Count == 0)
            {
                await AddNotificationActivityAsync(member.ProfileId, ActionType.AddNewItem, member.Id.ToString(), ParentType.AssessmentGroup, userId, notifyText, "");
            }
            //}


            return true;
        }

        public async Task<bool> MeetupNotification()
        {
            try { 
            var mainobj = await _mongoDbContext.NotificationGenericObjects.Find(_ => true).ToListAsync();


            foreach (var items in mainobj)
            {
                var noteObj = items.Notifications.Where(o => o.IsPushed == false && o.ParentTypeID == 10 && o.ActionID == 6);

                foreach (var item in noteObj)
                {
                    var meetup = await _appDbContext.Meetups.Where(x => x.Id == Convert.ToInt32(item.ParentID)).FirstOrDefaultAsync();
                        if(meetup != null)
                        {
                            var notifyText = "A new meeting" + " " + meetup.Title + " " + "has been added";

                                var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == items.UserID && x.CategoryID == (int)CategoryType.MeetingHub).FirstOrDefaultAsync();
                                if (customNotificationData?.isEnabled == true || customNotificationData == null)
                                {
                                    await AddOtherModulePushNotificationAsync(items.UserID, item, notifyText);
                                }
                        }
                    


                }
            }
            //var member = await _appDbContext.Meetups.OrderByDescending(x => x.Created).FirstOrDefaultAsync();
            ////var groupName = await _appDbContext.AssessmentGroups.Where(x => x.Id == member.AssessmentGroupId).Select(x => x.NameEn).FirstOrDefaultAsync();


            //var userId = await _appDbContext.UserInfos.Where(x => x.Email == member.CreatedBy).Select(x => x.UserId).FirstOrDefaultAsync();

            //var userIdMember = userId;
            //var postIdMember = member.Id.ToString();
            //var parentTypeIdMember = ParentType.Meetup;
            //var actionIdMember = ActionType.AddNewItem;
            ////var postTypeIdShare = postShare.TypeID;
            //var mongoUser = (await _mongoDbContext.Users.Find(k => k.Id == userId).FirstOrDefaultAsync()) ??
            //            new MongoModels.User() { };

            //var userIds = new List<int>();
            //userIds.AddRange(mongoUser.FollowersIDs);

            //var notifyText = "A new meeting has been added";
            //foreach (var follwingItem in userIds)
            //{
            //var notify = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == follwingItem).FirstOrDefaultAsync();

            //var Count = 0;
            //foreach (var item in notify.Notifications)
            //{
            //    if (item.SenderID == userIdMember && item.ParentID == postIdMember && item.ParentTypeID == (int)parentTypeIdMember && item.ActionID == (int)actionIdMember) //&& item.PostTypeID == 0
            //        {
            //        Count++; //1 // 0
            //    }
            //}
            //if (Count == 0)
            //{
            //        var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == follwingItem && x.CategoryID == (int)CategoryType.MeetingHub).FirstOrDefaultAsync();
            //        if (customNotificationData?.isEnabled == true || customNotificationData == null)
            //        {
            //            await AddNotificationActivityAsync(follwingItem, ActionType.AddNewItem, member.Id.ToString(), ParentType.Meetup, userId, notifyText, "");
            //        }
            //}
            //}


            return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        public async Task<bool> AddNotificationActivityAsync(int userId, ActionType actionId, string parentPostId, ParentType parentTypeId, int senderUserId, string notifyText, string roomId)
        {
            try
            {
                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId}");


                var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync() ??
                                                await AddNotificationObjectAsync(userId);



                var notificationObj = new Notification
                {
                    ID = ObjectId.GenerateNewId(),
                    ActionID = (int)actionId,
                    IsRead = false,
                    ParentID = parentPostId,
                    ParentTypeID = (int)parentTypeId,
                    SenderID = senderUserId,
                    IsPushed = true
                    //PostTypeID = typeId
                };

                notificationGenericObject.Notifications.Add(notificationObj);

                if (userId != senderUserId)
                {
                    notificationGenericObject.UnseenNotificationCounter += 1;
                    var notificationView = _mapper.Map<NotificationView>(notificationObj);
                    await FillNotificationUserDetailsAsync(userId, new List<NotificationView>() { notificationView }, roomId);

                    var deviceIds = await _appDbContext.UserDeviceInfos.Where(k => k.UserId == userId).Select(k => k.DeviceId).ToListAsync();
                    foreach (var deviceId in deviceIds)
                    {
                        await _pushNotificationService.SendRecommendPushNotificationAsync(notificationView, notifyText, deviceId);
                    }

                    //logger.Info("Notification sent");


                }

                await _mongoDbContext.NotificationGenericObjects.ReplaceOneAsync(x => x.UserID == userId, notificationGenericObject);

                var notificationGenericObjectView = new NotificationGenericObjectView
                {
                    ID = notificationGenericObject.ID.ToString(),
                    UserID = notificationGenericObject.UserID,
                    UnseenNotificationCounter = notificationGenericObject.UnseenNotificationCounter,
                    NotificationsList = _mapper.Map<List<NotificationView>>(notificationGenericObject.Notifications)
                };

                return true;
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw e;
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

        public async Task<bool> AddPushNotificationAsync(int userId, Notification obj)
        {
            try
            {
                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId}");


                var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync() ??
                                                await AddNotificationObjectAsync(userId);



                var Notifications = notificationGenericObject.Notifications.FirstOrDefault(o => o.ID == obj.ID && o.IsPushed == false);
                if (Notifications != null)
                {


                    obj.IsPushed = true;


                    // await _mongoDbContext.notificationGenericObject.ReplaceOneAsync(x => x.ID == view.MeetupId, genericObject);

                    if (userId != obj.SenderID)
                    {
                        notificationGenericObject.UnseenNotificationCounter += 1;
                        var notificationView = _mapper.Map<NotificationView>(obj);
                        await FillNotificationUserDetailsAsync(userId, new List<NotificationView>() { notificationView }, "");

                        //var deviceIds = await _appDbContext.UserDeviceInfos.Where(k => k.UserId == userId).Select(k => k.DeviceId).ToListAsync();
                        //foreach (var deviceId in deviceIds)
                        //{
                        //    await _pushNotificationService.SendPushNotificationAsync(notificationView, deviceId);
                        //}
                        PushNotification(notificationView, userId);
                        //logger.Info("Notification sent");

                    }
                    notificationGenericObject.Notifications.Remove(Notifications);
                    notificationGenericObject.Notifications.Add(obj);
                    await _mongoDbContext.NotificationGenericObjects.ReplaceOneAsync(x => x.UserID == userId, notificationGenericObject);
                }
                var notificationGenericObjectView = new NotificationGenericObjectView
                {
                    ID = notificationGenericObject.ID.ToString(),
                    UserID = notificationGenericObject.UserID,
                    UnseenNotificationCounter = notificationGenericObject.UnseenNotificationCounter,
                    NotificationsList = _mapper.Map<List<NotificationView>>(notificationGenericObject.Notifications)
                };

                return true;
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw e;
            }
        }

        public async Task<bool> PushNotification(NotificationView notification, int userId)
        {
            var deviceIds = await _appDbContext.UserDeviceInfos.Where(k => k.UserId == userId).Select(k => k.DeviceId).ToListAsync();
            foreach (var deviceId in deviceIds)
            {
                await _pushNotificationService.SendPushNotificationAsync(notification, deviceId);
            }
            return true;
        }

        public async Task<bool> AddOtherModulePushNotificationAsync(int userId, Notification obj, string notifyText)
        {
            try
            {
                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId}");


                var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync() ??
                                                await AddNotificationObjectAsync(userId);



                var Notifications = notificationGenericObject.Notifications.FirstOrDefault(o => o.ID == obj.ID && o.IsPushed == false);
                if (Notifications != null)
                {


                    obj.IsPushed = true;


                    // await _mongoDbContext.notificationGenericObject.ReplaceOneAsync(x => x.ID == view.MeetupId, genericObject);

                    if (userId != obj.SenderID)
                    {
                        notificationGenericObject.UnseenNotificationCounter += 1;
                        var notificationView = _mapper.Map<NotificationView>(obj);
                        await FillNotificationUserDetailsAsync(userId, new List<NotificationView>() { notificationView }, "");

                        //var deviceIds = await _appDbContext.UserDeviceInfos.Where(k => k.UserId == userId).Select(k => k.DeviceId).ToListAsync();
                        //foreach (var deviceId in deviceIds)
                        //{
                        //    await _pushNotificationService.SendRecommendPushNotificationAsync(notificationView, notifyText, deviceId);
                        //}
                        PushNotificationAdmin(notificationView, userId, notifyText);
                        //logger.Info("Notification sent");

                    }
                    notificationGenericObject.Notifications.Remove(Notifications);
                    notificationGenericObject.Notifications.Add(obj);
                    await _mongoDbContext.NotificationGenericObjects.ReplaceOneAsync(x => x.UserID == userId, notificationGenericObject);
                }
                var notificationGenericObjectView = new NotificationGenericObjectView
                {
                    ID = notificationGenericObject.ID.ToString(),
                    UserID = notificationGenericObject.UserID,
                    UnseenNotificationCounter = notificationGenericObject.UnseenNotificationCounter,
                    NotificationsList = _mapper.Map<List<NotificationView>>(notificationGenericObject.Notifications)
                };

                return true;
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw e;
            }
        }

        public async Task<bool> PushNotificationAdmin(NotificationView notification, int userId, string notifyText)
        {
            var deviceIds = await _appDbContext.UserDeviceInfos.Where(k => k.UserId == userId).Select(k => k.DeviceId).ToListAsync();
            foreach (var deviceId in deviceIds)
            {
                await _pushNotificationService.SendAdminPushNotificationAsync(notification, notifyText, deviceId);
            }
            return true;
        }

        private async Task FillNotificationUserDetailsAsync(int userId, List<NotificationView> notificationsList, string roomId)
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

                if (notification.ParentTypeID == ParentType.Challenge)
                {
                    var activity = await _appDbContext.Initiatives.Where(x => x.Id == Convert.ToInt32(notification.ParentID)).FirstOrDefaultAsync();
                    //notification.titleEn = activity?.TitleEn;
                    //notification.titleAr = activity?.TitleAr;
                    notification.categoryID = activity?.CategoryId != null ? activity?.CategoryId : activity?.InitiativeTypeItemId;
                }
                if (notification.ParentTypeID == ParentType.Messaging)
                {
                    var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();
                    notification.ParentID = room.ID.ToString();
                }
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
                logger.Error(e);
                throw e;
            }
        }

        //public async Task<bool> customNotification()
        //{
            
        //    return true;
        //}

    }
}
