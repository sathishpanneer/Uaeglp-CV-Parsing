using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.MongoModels;
using Uaeglp.Models;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;
using NLog;
using Uaeglp.Contract.Communication;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Serialization;
using MongoDB.Driver.GridFS;
using System.Globalization;
using Uaeglp.ViewModels.Meetup;
using Uaeglp.Utilities;

namespace Uaeglp.Services
{
    public class MessagingService : IMessagingService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly MongoDbContext _mongoDbContext;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IUserRecommendationService _userRecommendationService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IUserIPAddress _userIPAddress;

        public MessagingService(MongoDbContext mongoDbContext, IMapper mapper, AppDbContext appDbContext, IPushNotificationService pushNotificationService, IUserIPAddress userIPAddress)
        {
            _mongoDbContext = mongoDbContext;
            _appDbContext = appDbContext;
            _mapper = mapper;
            _pushNotificationService = pushNotificationService;
            _userIPAddress = userIPAddress;

        }


        public async Task<IMessagingResponse> GetSearchMessage(string searchText, int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {searchText} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

                var users = _appDbContext.Users.Where(x => x.NameAr.Trim().ToLower().Contains(searchText.Trim().ToLower()) ||
                x.NameEn.Trim().ToLower().Contains(searchText.Trim().ToLower())).ToList();

                var messagesRecommendationView = new List<MessagesRecommendationView>();
                var RoomsModelView = new List<RoomViewModel>();
                //var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();
                var rooms = await (await _mongoDbContext.Rooms.FindAsync(x => x.MembersIDs.Contains(userId) || x.OwnerID == userId)).ToListAsync();
                
                foreach (Room room in rooms)
                {
                    RoomViewModel messageViewModel = new RoomViewModel()
                    {
                        ID = room.ID.ToString(),
                        OwnerID = room.OwnerID,
                        ArchivedMembersIDs = room.ArchivedMembersIDs,
                        CreatedOn = room.CreatedOn,
                        LastModifiedOn = room.LastModifiedOn,
                        //MembersIDs = room.MembersIDs,
                        ModifiedBy = room.ModifiedBy,
                        NumberOfMembers = room.NumberOfMembers,
                        RoomTitle = room.RoomTitle,
                        RoomTypeID = room.RoomTypeID,
                        UnreadMessages = _mapper.Map<IList<UnreadMessageView>>(room.UnreadMessages)
                    };

                    List<MemberModel> _memberList = new List<MemberModel>();
                    foreach (var item in room.MembersIDs)
                    {
                        MemberModel _members = new MemberModel();
                        var Memuser = _appDbContext.Users.Where(x => x.Id == item).FirstOrDefault();
                        var MemDesignation = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title).Where(k => k.ProfileId == item).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();
                        _members.MembersID = item;
                        if (Memuser != null)
                        {
                            _members.NameEn = (Memuser.NameEn != null) ? Memuser.NameEn : "";
                            _members.NameAr  = (Memuser.NameAr != null) ? Memuser.NameAr : "";
                            _members.DesignationEn = MemDesignation?.Title?.TitleEn;
                            _members.DesignationAr = MemDesignation?.Title?.TitleAr;
                            _members.MemberImageFileId = (Memuser.OriginalImageFileId != null) ? Memuser.OriginalImageFileId : 0;
                        }
                        _memberList.Add(_members);
                    }
                    messageViewModel.MembersIDs = _memberList;
                    var RoomOwnerName = await _appDbContext.Users.Where(x => x.Id == messageViewModel.OwnerID).FirstOrDefaultAsync();
                    RecipientUserView Oview = new RecipientUserView();
                    if (RoomOwnerName != null)
                    {
                        Oview.RecipientUserId = messageViewModel.OwnerID;
                        Oview.RecipientName = new MeetupLangView() { En = RoomOwnerName.NameEn, Ar = RoomOwnerName.NameAr };
                        var RprofileDesignation = _appDbContext.ProfileWorkExperiences.Include(k => k.Title).FirstOrDefault(k => k.ProfileId == RoomOwnerName.Id);
                        if (RprofileDesignation != null)
                        {
                            Oview.Designation = new MeetupLangView() { En = RprofileDesignation.Title?.TitleEn, Ar = RprofileDesignation.Title?.TitleAr };
                        }
                        Oview.RecipientUserImageId = RoomOwnerName.SmallImageFileId;

                        messageViewModel.RoomOwnerInfo = Oview;
                    }
                    List<Message> data = new List<Message>();
                    var Messagedata = room.Messages.Where(x => !string.IsNullOrEmpty(x.MessageText) ? 
                            x.MessageText.Trim().ToLower().Contains(searchText.Trim().ToLower(), StringComparison.OrdinalIgnoreCase) : false).ToList();
                    data.AddRange(Messagedata);
                    foreach (var item in users)
                    {
                        var senderdata = room.Messages.Where(x => x.OwnerID == item.Id).ToList();
                        if (senderdata.Count > 0)
                        {
                            data.AddRange(senderdata);
                        }
                    }

                        if (room.RoomTitle != null && room.RoomTitle.Trim().ToLower().Contains(searchText.Trim().ToLower()))
                        {

                            var roomTitleData = room.Messages.ToList();
                            if (roomTitleData.Count > 0)
                            {
                                data.AddRange(roomTitleData);
                            }
                        }

                    var message = new List<MessageViewModel>();
                    foreach (Message item in data.Distinct())
                    {

                        MessageViewModel messageView = new MessageViewModel();
                        messageView.ID = item.ID.ToString();
                        messageView.MessageText = item.MessageText;
                        messageView.OwnerID = item.OwnerID;
                        messageView.SeenByIDs = item.SeenByIDs;
                        messageView.ImagesIDs = item.ImagesIDs;
                        messageView.FilesIDs = item.FilesIDs;
                        messageView.TypeID = item.TypeID;
                        messageView.Created = item.Created;

                        var recepientName = await _appDbContext.Users.Where(x => x.Id == item.OwnerID).FirstOrDefaultAsync();
                        RecipientUserView rview = new RecipientUserView();
                        if (recepientName != null)
                        {
                            rview.RecipientUserId = item.OwnerID;
                            rview.RecipientName = new MeetupLangView() { En = recepientName.NameEn, Ar = recepientName.NameAr };
                            var profileDesignation = _appDbContext.ProfileWorkExperiences.Include(k => k.Title).FirstOrDefault(k => k.ProfileId == recepientName.Id);
                            if (profileDesignation != null)
                            {
                                rview.Designation = new MeetupLangView() { En = profileDesignation.Title?.TitleEn, Ar = profileDesignation.Title?.TitleAr };
                            }
                            rview.RecipientUserImageId = recepientName.SmallImageFileId;

                            messageView.OwnerInfo = rview;
                        }
                        message.Add(messageView);
                    }
                    if (message.Count > 0)
                    {
                        messageViewModel.Messages = message;
                        RoomsModelView.Add(messageViewModel);
                    }
                    
                }

                foreach (var item in RoomsModelView)
                {
                    MessagesRecommendationView _view = new MessagesRecommendationView();
                    _view.UserMessage = item;
                    _view.LastUpdated = item.LastModifiedOn;
                    messagesRecommendationView.Add(_view);

                }

                var userRecommendView = await ReceiveAllRecommendationAsync(userId);
                var usernamerecommend = new List<UserRecommendView>();
                foreach (var item in users)
                {
                    usernamerecommend.AddRange(userRecommendView.Where(x => x.SenderUserID == item.Id).ToList());
                }
                var searchRecommendview =  userRecommendView.Where(x => x.RecommendationText.Trim().ToLower().Contains(searchText.Trim().ToLower())).ToList();
                usernamerecommend.AddRange(searchRecommendview);
                
                foreach (var item in usernamerecommend.Distinct())
                {
                    MessagesRecommendationView _view = new MessagesRecommendationView();
                    _view.UserRecommend = item;
                    _view.LastUpdated = item.Modified;
                    messagesRecommendationView.Add(_view);

                }

                var roomView = messagesRecommendationView.OrderByDescending(s => s.LastUpdated).ToList();

                return new MessagingResponse(roomView);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new MessagingResponse(e);
            }

        }


        public async Task<IMessagingResponse> GetRecepientName(string searchName)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {searchName} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                List<RecipientUserView> _view = new List<RecipientUserView>();
            var recepientName = await _appDbContext.Users.Where(x => x.NameEn.StartsWith(searchName)).ToListAsync();
                
                foreach (var item in recepientName)
                {
                    RecipientUserView rview = new RecipientUserView();
                    rview.RecipientUserId = item.Id;
                   rview.RecipientName = new MeetupLangView() { En = item.NameEn, Ar = item.NameAr };
                    var profileDesignation = _appDbContext.ProfileWorkExperiences.Include(k => k.Title).FirstOrDefault(k => k.ProfileId == item.Id);
                    if (profileDesignation != null)
                    {
                        rview.Designation = new MeetupLangView() { En = profileDesignation.Title?.TitleEn, Ar = profileDesignation.Title?.TitleAr };
                    }
                    rview.RecipientUserImageId = item.SmallImageFileId;
                    _view.Add(rview);
                }

             
            return new MessagingResponse(_view);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new MessagingResponse(e);
            }
        }

        public async Task<IMessagingResponse> GetNotificationCount(int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

                int Mcount = 0;
            var rooms = await _mongoDbContext.Rooms.Find(x => x.MembersIDs.Contains(userId)).ToListAsync();
            var MsgCount = rooms.Select(x => x.UnreadMessages.FirstOrDefault().MessagesCount);
            foreach (Room room in rooms)
            {

                int cnt = room.UnreadMessages.Where(x => x.UserID == userId).Select(x=>x.MessagesCount).FirstOrDefault();
                Mcount = Mcount + cnt;
                    
            }
            Mcount = Mcount + _appDbContext.UserRecommendations.Count(x => x.RecipientUserID == userId && x.isRead == false);

            return new MessagingResponse(Mcount);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new MessagingResponse(e);
            }
        }
        public async Task<IMessagingResponse> GetRoomListAsync(int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                //var rooms = await _mongoDbContext.Rooms.Find(x => x.MembersIDs.Contains(userId)).ToListAsync();
                var messagesRecommendationView = new List<MessagesRecommendationView>();
                
                var RoomsModelView = new List<RoomViewModel>();
                //var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();
                var rooms = await _mongoDbContext.Rooms.Find(x => x.MembersIDs.Contains(userId)).ToListAsync();
                if (rooms != null)
                {
                    foreach (Room room in rooms)
                    {
                        var message = new List<MessageViewModel>();
                        RoomViewModel messageViewModel = new RoomViewModel()
                        {
                            ID = room.ID.ToString(),
                            OwnerID = room.OwnerID,
                            ArchivedMembersIDs = room.ArchivedMembersIDs,
                            CreatedOn = room.CreatedOn,
                            LastModifiedOn = room.LastModifiedOn,
                            //MembersIDs = room.MembersIDs,
                            ModifiedBy = room.ModifiedBy,
                            NumberOfMembers = room.NumberOfMembers,
                            RoomTitle = room.RoomTitle,
                            RoomTypeID = room.RoomTypeID,
                            UnreadMessages = _mapper.Map<IList<UnreadMessageView>>(room.UnreadMessages)
                        };

                        List<MemberModel> _memberList = new List<MemberModel>();
                        foreach (var item1 in room.MembersIDs)
                        {
                            MemberModel _members = new MemberModel();
                            var Memuser = _appDbContext.Users.Where(x => x.Id == item1).FirstOrDefault();
                            var MemDesignation = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title).Where(k => k.ProfileId == item1).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();

                            _members.MembersID = item1;
                            if (Memuser != null)
                            {
                                _members.NameEn = (Memuser.NameEn != null) ? Memuser.NameEn : "";
                                _members.NameAr = (Memuser.NameAr != null) ? Memuser.NameAr : "";
                                _members.DesignationEn = MemDesignation?.Title?.TitleEn;
                                _members.DesignationAr = MemDesignation?.Title?.TitleAr;
                                _members.MemberImageFileId = (Memuser.OriginalImageFileId != null) ? Memuser.OriginalImageFileId : 0;

                            }
                            _memberList.Add(_members);
                        }
                        messageViewModel.MembersIDs = _memberList;


                        var RoomOwnerName = await _appDbContext.Users.Where(x => x.Id == messageViewModel.OwnerID).FirstOrDefaultAsync();
                        RecipientUserView Oview = new RecipientUserView();
                        if (RoomOwnerName != null)
                        {
                            Oview.RecipientUserId = messageViewModel.OwnerID;
                            Oview.RecipientName = new MeetupLangView() { En = RoomOwnerName.NameEn, Ar = RoomOwnerName.NameAr };
                            var RprofileDesignation = _appDbContext.ProfileWorkExperiences.Include(k => k.Title).FirstOrDefault(k => k.ProfileId == RoomOwnerName.Id);
                            if (RprofileDesignation != null)
                            {
                                Oview.Designation = new MeetupLangView() { En = RprofileDesignation.Title?.TitleEn, Ar = RprofileDesignation.Title?.TitleAr };
                            }
                            Oview.RecipientUserImageId = RoomOwnerName.SmallImageFileId;

                            messageViewModel.RoomOwnerInfo = Oview;
                        }
                        var item = room.Messages.OrderByDescending(x => x.Created).FirstOrDefault();
                        if (item != null)
                        {

                            MessageViewModel messageView = new MessageViewModel();
                            messageView.ID = item.ID.ToString();
                            messageView.MessageText = item.MessageText;
                            messageView.OwnerID = item.OwnerID;
                            messageView.SeenByIDs = item.SeenByIDs;
                            messageView.ImagesIDs = item.ImagesIDs;
                            messageView.FilesIDs = item.FilesIDs;
                            messageView.TypeID = item.TypeID;
                            messageView.Created = item.Created;

                            var recepientName = await _appDbContext.Users.Where(x => x.Id == item.OwnerID).FirstOrDefaultAsync();
                            RecipientUserView rview = new RecipientUserView();
                            if (recepientName != null)
                            {
                                rview.RecipientUserId = item.OwnerID;
                                rview.RecipientName = new MeetupLangView() { En = recepientName.NameEn, Ar = recepientName.NameAr };
                                var profileDesignation = _appDbContext.ProfileWorkExperiences.Include(k => k.Title).FirstOrDefault(k => k.ProfileId == recepientName.Id);
                                if (profileDesignation != null)
                                {
                                    rview.Designation = new MeetupLangView() { En = profileDesignation.Title?.TitleEn, Ar = profileDesignation.Title?.TitleAr };
                                }
                                rview.RecipientUserImageId = recepientName.SmallImageFileId;

                                messageView.OwnerInfo = rview;
                            }
                            message.Add(messageView);
                        }
                        else
                        {

                        }
                        messageViewModel.Messages = message;
                        RoomsModelView.Add(messageViewModel);
                    }
                    //var msgCount = rooms.Select(x => x.UnreadMessages.FirstOrDefault().MessagesCount);
                }
                foreach (var item in RoomsModelView)
                {
                    MessagesRecommendationView _view = new MessagesRecommendationView();
                    _view.UserMessage = item;
                    _view.LastUpdated = item.LastModifiedOn;
                    messagesRecommendationView.Add(_view);

                }

                var userRecommendView = await ReceiveAllRecommendationAsync(userId);
                foreach (var item in userRecommendView)
                {
                    MessagesRecommendationView _view = new MessagesRecommendationView();
                    _view.UserRecommend = item;
                    _view.LastUpdated = item.Modified;
                    messagesRecommendationView.Add(_view);

                }

                var roomView = messagesRecommendationView.OrderByDescending(s=> s.LastUpdated).ToList();
                return new MessagingResponse(roomView);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new MessagingResponse(e);
            }
        }
        private async Task<List<UserRecommendView>> ReceiveAllRecommendationAsync(int recipientUserId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {recipientUserId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

                List<UserRecommendView> _details = new List<UserRecommendView>();
                var data = await _appDbContext.UserRecommendations.Where(x => x.RecipientUserID == recipientUserId && x.isAccepted != true).ToListAsync();

                var recommendDetails = _mapper.Map<List<UserRecommendView>>(data);

                if (recommendDetails != null)
                {



                    foreach (var item in recommendDetails)
                    {
                        var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == item.SenderUserID);
                        var workExperience = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title)
                            .Where(k => k.ProfileId == item.SenderUserID).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();
                        var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == item.SenderUserID);
                        item.UserInfo = new PublicProfileView()
                        {
                            Id = item.SenderUserID,
                            FirstNameAr = profile?.FirstNameAr ?? "",
                            FirstNameEn = profile?.FirstNameEn ?? "",
                            LastNameAr = profile?.LastNameAr ?? "",
                            LastNameEn = profile?.LastNameEn ?? "",
                            SecondNameAr = profile?.SecondNameAr ?? "",
                            SecondNameEn = profile?.SecondNameEn ?? "",
                            ThirdNameAr = profile?.ThirdNameAr ?? "",
                            ThirdNameEn = profile?.ThirdNameEn ?? "",
                            Designation = workExperience?.Title?.TitleEn ?? "",
                            DesignationAr = workExperience?.Title?.TitleAr ?? "",
                            UserImageFileId = user?.OriginalImageFileId ?? 0,
                            About = ""
                        };
                        _details.Add(item);
                    }
                }
                
                return _details;

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        public async Task<IMessagingResponse> GetRoomAsync(string roomId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {roomId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();

                if (room == null) return new MessagingResponse(ClientMessageConstant.UserNotFound, HttpStatusCode.NotFound);
                var roomView =  _mapper.Map<RoomView>(room);
                return new MessagingResponse(roomView);
            }
            catch (Exception e)
            {
                return new MessagingResponse(e);
            }
        }
        public async Task<IMessagingResponse> CreateRoomAndMessageAsync(RoomAndMessageCreateView view)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {view.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                RoomView _room = new RoomView();
                string _id;
                //var room = _mapper.Map<Room>(view);
                //await _mongoDbContext.Rooms.InsertOneAsync(room);
                ///foreach(var item in view.MembersIDs)
                ///{
                // view.MembersIDs.Add(item);
                char[] spearator = { '|' };
                var Ids = view.MembersIDs.Split(spearator);
                List<int> memers = new List<int>();
                foreach (var item in Ids)
                {
                    if (item != "")
                    {
                        memers.Add(Convert.ToInt32(item));
                    }

                }
                var roomType = memers.Count <= 2 ? 1 : 2;
                //}

                //var room = await _mongoDbContext.Rooms.Find(x => x.MembersIDs.Count == 2 && x.RoomTypeID == 1 && x.MembersIDs.Contains(memers[0]) && x.OwnerID == view.userId).FirstOrDefaultAsync();
                var room = await _mongoDbContext.Rooms.Find(x => x.MembersIDs.Count == 2 && x.RoomTypeID == 1
              && x.MembersIDs == memers && x.MembersIDs.Contains(view.userId)).FirstOrDefaultAsync();
                if (memers.Count == 2 && room != null)
                {
                    _room =  GetRoom(room.ID.ToString(), view.userId).Result.Room;
                    _id = _room.ID;
                }
                else
                {
                    List<UnreadMessage> unreadMessageList = new List<UnreadMessage>();
                    foreach (var membersId in Ids)
                    {
                        if (membersId != "")
                        {
                            unreadMessageList.Add(new UnreadMessage()
                            {
                                UserID = Convert.ToInt32(membersId),
                                MessagesCount = 0
                            });
                        }
                    }
                    var roomId = ObjectId.GenerateNewId();
                    var firstName = await _appDbContext.Profiles.Where(k => k.Id == view.userId).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
                    var lastName = await _appDbContext.Profiles.Where(k => k.Id == view.userId).Select(k => k.LastNameEn).FirstOrDefaultAsync();
                    var userName = firstName + " " + lastName;
                   
                    var messages = new Room()
                    {
                        ID = roomId,
                        CreatedOn = DateTime.Now,
                        LastModifiedOn = DateTime.Now,
                        OwnerID = view.userId,
                        MembersIDs = memers,
                        RoomTypeID = roomType,
                        RoomTitle = view.RoomTitle,
                        UnreadMessages = unreadMessageList,
                        ModifiedBy = userName

                    };

                    await _mongoDbContext.Rooms.InsertOneAsync(messages);
                    _id = roomId.ToString();
                   //_room = GetRoom(roomId.ToString(), view.userId).Result.Room;
                }

                var message = await AddRoomMessageAsync(new MessageAddView()
                {
                    userId = view.userId,
                    RoomId = _id,
                    MessageText = view.MessageText,
                    DocumentData = view.DocumentData,
                    ImageData = view.ImageData
                });

                return await GetRoomMessageAsync(_id);
                //return new MessagingResponse(_mapper.Map<RoomView>(room));
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new MessagingResponse(e);
            }
        }
        public async Task<IMessagingResponse> AddRoomAsync(AddRoomView view)
        {

            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {view.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                //var room = _mapper.Map<Room>(view);
                //await _mongoDbContext.Rooms.InsertOneAsync(room);
                ///foreach(var item in view.MembersIDs)
                ///{
                // view.MembersIDs.Add(item);
                var roomType = view.MembersIDs.Count <= 2 ? 1 : 2;
                //}

                List<UnreadMessage> unreadMessageList = new List<UnreadMessage>();
                List<int> _IdsList = new List<int>();
                foreach (var membersId in view.MembersIDs)
                {
                    unreadMessageList.Add(new UnreadMessage()
                    {
                        UserID = membersId,
                        MessagesCount = 0
                    });
                    
                }

                var room = await _mongoDbContext.Rooms.Find(x => x.MembersIDs.Count == 2 && x.RoomTypeID == 1 
                && x.MembersIDs.Contains(view.MembersIDs[0]) && x.MembersIDs.Contains(view.MembersIDs[1]) && x.MembersIDs.Contains(view.userId)).FirstOrDefaultAsync();
                //groupIds.Any(x => groupIds.Any(y => y == x))

                if (view.MembersIDs.Count == 2 && room != null)
                    return await GetRoom(room.ID.ToString(), view.userId);

               
                var roomId = ObjectId.GenerateNewId();
                var firstName = await _appDbContext.Profiles.Where(k => k.Id == view.userId).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
                var lastName = await _appDbContext.Profiles.Where(k => k.Id == view.userId).Select(k => k.LastNameEn).FirstOrDefaultAsync();
                var userName = firstName + " " + lastName;
                var messages = new Room()
                {
                    ID = roomId,
                    CreatedOn = DateTime.Now,
                    LastModifiedOn = DateTime.Now,
                    OwnerID = view.userId,
                    MembersIDs = view.MembersIDs,
                    RoomTypeID = roomType,
                    RoomTitle = view.RoomTitle,
                    UnreadMessages = unreadMessageList,
                    ModifiedBy = userName

                };

                await _mongoDbContext.Rooms.InsertOneAsync(messages);



                return await GetRoom(roomId.ToString(), view.userId);
                //return new MessagingResponse(_mapper.Map<RoomView>(room));
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new MessagingResponse(e);
            }
        }

        public async Task<IMessagingResponse> UpdateRoomAsync(UpdateRommView view)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {view.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(view.roomID)).FirstOrDefaultAsync();
                var firstName = await _appDbContext.Profiles.Where(k => k.Id == view.userId).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
                var lastName = await _appDbContext.Profiles.Where(k => k.Id == view.userId).Select(k => k.LastNameEn).FirstOrDefaultAsync();
                var userName = firstName + " " + lastName;

                room.RoomTitle = view.RoomTitle;
                room.LastModifiedOn = DateTime.Now;
                room.ModifiedBy = userName;
                await _mongoDbContext.Rooms.ReplaceOneAsync(x => x.ID == new ObjectId(view.roomID), room);

                var roomView = _mapper.Map<RoomView>(room);

                List<MemberModel> _memberList = new List<MemberModel>();
                foreach (var item1 in room.MembersIDs)
                {
                    MemberModel _members = new MemberModel();
                    var Memuser = _appDbContext.Users.Where(x => x.Id == item1).FirstOrDefault();
                    var MemDesignation = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title).Where(k => k.ProfileId == item1).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();

                    _members.MembersID = item1;
                    if (Memuser != null)
                    {
                        _members.NameEn = (Memuser.NameEn != null) ? Memuser.NameEn : "";
                        _members.NameAr = (Memuser.NameAr != null) ? Memuser.NameAr : "";
                        _members.DesignationEn = MemDesignation?.Title?.TitleEn;
                        _members.DesignationAr = MemDesignation?.Title?.TitleAr;
                        _members.MemberImageFileId = (Memuser.OriginalImageFileId != null) ? Memuser.OriginalImageFileId : 0;

                    }
                    _memberList.Add(_members);
                }
                roomView.MemberDetails = _memberList;
                return new MessagingResponse(roomView);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new MessagingResponse(ex);
            }
    
           
        }

        public async Task<IMessagingResponse> GetRoom(string roomId, int userId)
        {

            var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();

            //var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == item.Id);
            //var workExperience = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title)
            //    .Where(k => k.ProfileId == item.Id).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();
            //var userDetails = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == item.Id);

            if (!room.MembersIDs.Contains(userId))
                room.MembersIDs.Add(userId);

            var data = new Room()
            {
                ID = room.ID,
                CreatedOn = room.CreatedOn,
                LastModifiedOn = room.LastModifiedOn,
                RoomTitle = room.RoomTitle,
                NumberOfMembers = room.MembersIDs.Count<int>(),
                //Members = this._profileLogic.GetMultiple(room.MembersIDs),
                RoomTypeID = room.RoomTypeID,
                MembersIDs = room.MembersIDs,
                UnreadMessages = room.UnreadMessages.Select(x => new UnreadMessage()
                {
                    MessagesCount = x.MessagesCount,
                    UserID = x.UserID,
                    //UserProfile = this._profileLogic.GetProfile(u.UserID)
                }).ToList()
            };
           // return new MessagingResponse(_mapper.Map<RoomView>(data));

            var roomView = _mapper.Map<RoomView>(data);

            List<MemberModel> _memberList = new List<MemberModel>();
            foreach (var item1 in room.MembersIDs)
            {
                MemberModel _members = new MemberModel();
                var Memuser = _appDbContext.Users.Where(x => x.Id == item1).FirstOrDefault();
                var MemDesignation = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title).Where(k => k.ProfileId == item1).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();

                _members.MembersID = item1;
                if (Memuser != null)
                {
                    _members.NameEn = (Memuser.NameEn != null) ? Memuser.NameEn : "";
                    _members.NameAr = (Memuser.NameAr != null) ? Memuser.NameAr : "";
                    _members.DesignationEn = MemDesignation?.Title?.TitleEn;
                    _members.DesignationAr = MemDesignation?.Title?.TitleAr;
                    _members.MemberImageFileId = (Memuser.OriginalImageFileId != null) ? Memuser.OriginalImageFileId : 0;

                }
                _memberList.Add(_members);
            }
            roomView.MemberDetails = _memberList;
            return new MessagingResponse(roomView);

        }

        public async Task<IMessagingResponse> GetRoomMessageAsync(string roomId)
        {
            try
            {

                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {roomId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                List<MessageViewModel> message = new List<MessageViewModel>();
                //RoomViewModel messageViewModel = new RoomViewModel();
                //messageViewModel.m = (int)this._mongoCntxt.Messages.Count();

                var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();
                RoomViewModel messageViewModel = new RoomViewModel()
                {
                    ID = room.ID.ToString(),
                    OwnerID = room.OwnerID,
                    ArchivedMembersIDs = room.ArchivedMembersIDs,
                    CreatedOn = room.CreatedOn,
                    LastModifiedOn = room.LastModifiedOn,
                   // MembersIDs = room.MembersIDs,
                    ModifiedBy = room.ModifiedBy,
                    NumberOfMembers = room.NumberOfMembers,
                    RoomTitle = room.RoomTitle,
                    RoomTypeID = room.RoomTypeID,
                    UnreadMessages = _mapper.Map<IList<UnreadMessageView>>(room.UnreadMessages)
                };

                List<MemberModel> _memberList = new List<MemberModel>();
                foreach (var item1 in room.MembersIDs)
                {
                    MemberModel _members = new MemberModel();
                    var Memuser = _appDbContext.Users.Where(x => x.Id == item1).FirstOrDefault();
                    var MemDesignation = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title).Where(k => k.ProfileId == item1).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();
                    _members.MembersID = item1;
                    if (Memuser != null)
                    {
                        _members.NameEn = (Memuser.NameEn != null) ? Memuser.NameEn : "";
                        _members.NameAr = (Memuser.NameAr != null) ? Memuser.NameAr : "";
                        _members.DesignationEn = MemDesignation?.Title?.TitleEn;
                        _members.DesignationAr = MemDesignation?.Title?.TitleAr;
                        _members.MemberImageFileId = (Memuser.OriginalImageFileId != null) ? Memuser.OriginalImageFileId : 0;
                    }
                    _memberList.Add(_members);
                }
                messageViewModel.MembersIDs = _memberList;

                var RoomOwnerName = await _appDbContext.Users.Where(x => x.Id == messageViewModel.OwnerID).FirstOrDefaultAsync();
                RecipientUserView Oview = new RecipientUserView();
                if (RoomOwnerName != null)
                {
                    Oview.RecipientUserId = messageViewModel.OwnerID;
                    Oview.RecipientName = new MeetupLangView() { En = RoomOwnerName.NameEn, Ar = RoomOwnerName.NameAr };
                    var RprofileDesignation = _appDbContext.ProfileWorkExperiences.Include(k => k.Title).FirstOrDefault(k => k.ProfileId == RoomOwnerName.Id);
                    if (RprofileDesignation != null)
                    {
                        Oview.Designation = new MeetupLangView() { En = RprofileDesignation.Title?.TitleEn, Ar = RprofileDesignation.Title?.TitleAr };
                    }
                    Oview.RecipientUserImageId = RoomOwnerName.SmallImageFileId;

                    messageViewModel.RoomOwnerInfo = Oview;
                }
                foreach (var item in room.Messages)
                {
                    MessageViewModel messageView = new MessageViewModel();
                    messageView.ID = item.ID.ToString();
                    messageView.MessageText = item.MessageText;
                    messageView.OwnerID = item.OwnerID;
                    messageView.SeenByIDs = item.SeenByIDs;
                    messageView.ImagesIDs = item.ImagesIDs;
                    messageView.FilesIDs = item.FilesIDs;
                    messageView.TypeID = item.TypeID;
                    messageView.Created = item.Created;

                    var recepientName = await _appDbContext.Users.Where(x => x.Id == item.OwnerID).FirstOrDefaultAsync();
                    RecipientUserView rview = new RecipientUserView();
                    if (recepientName != null)
                    {
                        rview.RecipientUserId = item.OwnerID;
                        rview.RecipientName = new MeetupLangView() { En = recepientName.NameEn, Ar = recepientName.NameAr };
                        var profileDesignation = _appDbContext.ProfileWorkExperiences.Include(k => k.Title).FirstOrDefault(k => k.ProfileId == recepientName.Id);
                        if (profileDesignation != null)
                        {
                            rview.Designation = new MeetupLangView() { En = profileDesignation.Title?.TitleEn, Ar = profileDesignation.Title?.TitleAr };
                        }
                        rview.RecipientUserImageId = recepientName.SmallImageFileId;

                        messageView.OwnerInfo = rview;
                    }
                    message.Add(messageView);
                }

                //messageViewModel.Messages = _mapper.Map<MessageViewModel>(source.ToList<Message>());
                //messageViewModel.Room.RoomTitle = string.IsNullOrEmpty(this._mongoCntxt.Rooms.FindOneById((BsonValue)roomId).RoomTitle) ? this.GenerateRoomName(this._mongoCntxt.Rooms.FindOneById((BsonValue)roomId).MembersIDs) : this._mongoCntxt.Rooms.FindOneById((BsonValue)roomId).RoomTitle;
                //messageViewModel = this._mongoCntxt.Rooms.FindOneById((BsonValue)roomId).MembersIDs.Count;
                //messageViewModel.r = this._mongoCntxt.Rooms.FindOneById((BsonValue)roomId).RoomTypeID;
                //messageViewModel.Room.ID = roomId;
                //messageViewModel.PagingDTO = pagingDTO;


                messageViewModel.Messages = message;

                return new MessagingResponse(messageViewModel);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new MessagingResponse(e);
            }
        }
        public async Task<IMessagingResponse> DeleteRoomAsync(string roomId)
        {
            try
            {
                await _mongoDbContext.Rooms.DeleteOneAsync(x => x.ID == new ObjectId(roomId));
                return new MessagingResponse();
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new MessagingResponse(e);
            }
        }

        public async Task<IMessagingResponse> AddRoomMemberAsync(string roomId, int userId)
        {
            try
            {
                var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();
                room.MembersIDs.Add(userId);
                await _mongoDbContext.Rooms.ReplaceOneAsync(x => x.ID == new ObjectId(roomId), room);
                return new MessagingResponse(_mapper.Map<RoomView>(room));
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new MessagingResponse(e);
            }
        }

        public async Task<IMessagingResponse> DeleteRoomMemberAsync(string roomId, int userId)
        {
            try
            {
                var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();
                if (room.MembersIDs == null) return new MessagingResponse(_mapper.Map<RoomView>(room));

                room.MembersIDs.Remove(userId);
                await _mongoDbContext.Rooms.ReplaceOneAsync(x => x.ID == new ObjectId(roomId), room);
                return new MessagingResponse(_mapper.Map<RoomView>(room));
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new MessagingResponse(e);
            }
        }

        public async Task<IMessagingResponse> AddRoomMessageAsync(MessageAddView message)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {message.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                List<string> imageList = new List<string>();
                List<string> documentList = new List<string>();
                var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(message.RoomId)).FirstOrDefaultAsync();
                var firstName = await _appDbContext.Profiles.Where(k => k.Id == message.userId).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
                var lastName = await _appDbContext.Profiles.Where(k => k.Id == message.userId).Select(k => k.LastNameEn).FirstOrDefaultAsync();
                var userName = firstName + " " + lastName;
                var imageId = ObjectId.GenerateNewId();
                var documentId = ObjectId.GenerateNewId();

                int TypeID = 1;
                if(message.ImageData != null)
                {
                    TypeID = 2;
                    imageList.Add(imageId.ToString());
                }
                if (message.DocumentData != null)
                {
                    TypeID = 3;
                    documentList.Add(documentId.ToString());
                }
               // message.SeenByIDs.Add(userId);
                var messageId = ObjectId.GenerateNewId();
                var fileName = "";
                var bucket = new GridFSBucket(_mongoDbContext.Database);
                var messages = new Message()
                {
                    ID = messageId,
                    MessageText = message.MessageText,
                    Created = DateTime.Now,
                    OwnerID = message.userId,
                    TypeID = TypeID,
                    //SeenByIDs = message.SeenByIDs,
                    FilesIDs = TypeID == 3 ? documentList : new List<string>(),
                    ImagesIDs = TypeID == 2 ? imageList : new List<string>()

                };
                room.LastModifiedOn = DateTime.Now;
                room.ModifiedBy = userName;

                foreach (UnreadMessage unreadMessage in room.UnreadMessages)
                {
                    if (unreadMessage.UserID != message.userId)
                        ++unreadMessage.MessagesCount;
                }

                if(message.ImageData != null)
                {
                    fileName = message.ImageData.FileName;
                    await PostImageUploadAsync(message, bucket, messages);
                }

                if (message.DocumentData != null)
                {
                    fileName = message.DocumentData.FileName;
                    await PostDocumentUploadAsync(message, bucket, messages);
                }

                room.Messages.Add(_mapper.Map<Message>(messages));
                await _mongoDbContext.Rooms.ReplaceOneAsync(x => x.ID == new ObjectId(message.RoomId), room);

                var userIds = new List<int>();
                foreach(var item in room.MembersIDs)
                {
                    if(item != message.userId)
                    {
                        userIds.Add(item);
                    }
                }
                var notifyText = userName + " " + "has sent a message";
                foreach (var recipientUserID in userIds)
                {
                    var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == recipientUserID && x.CategoryID == (int)CategoryType.Messages).FirstOrDefaultAsync();
                    if (customNotificationData?.isEnabled == true || customNotificationData == null)
                    {
                        await AddNotificationAsync(recipientUserID, ActionType.AddNewItem, messages.ID.ToString(), ParentType.Messaging, messages.OwnerID, notifyText, room.ID.ToString());
                    }
                }
                

                return await GetMessage(message.RoomId, messageId.ToString(), fileName);
                
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new MessagingResponse(e);
            }
        }

        public async Task<IUserRecommendationResponse> AddNotificationAsync(int recipientUserID, ActionType actionId, string messageId, ParentType parentTypeId, int senderUserId, string notifyText, string roomId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {senderUserId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

                //var profileId = userId != senderUserId ? senderUserId : userId;
                var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == recipientUserID).FirstOrDefaultAsync() ??
                                                await AddNotificationObjectAsync(recipientUserID);

                var notificationObj = new Notification
                {
                    ID = ObjectId.GenerateNewId(),
                    ActionID = (int)actionId,
                    IsRead = false,
                    ParentID = messageId,
                    ParentTypeID = (int)parentTypeId,
                    SenderID = senderUserId
                };

                notificationGenericObject.Notifications.Add(notificationObj);

                //if (userId != senderUserId)
                //{
                notificationGenericObject.UnseenNotificationCounter += 1;
                var notificationView = _mapper.Map<NotificationView>(notificationObj);
                await FillNotificationUserDetailsAsync(recipientUserID, new List<NotificationView>() { notificationView }, roomId);

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
                throw e;
            }
        }

        private async Task<IMessagingResponse> GetMessage(string roomId, string messageId, string fileName)
        {
            var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();
            var message = room.Messages.FirstOrDefault(x => x.ID ==  new ObjectId(messageId));

            var getmsg =  new MessageViewModel()
            {
                ID = message.ID.ToString(),
                MessageText = message.MessageText,
                OwnerID = message.OwnerID,
                SeenByIDs = message.SeenByIDs,
                ImagesIDs = message.ImagesIDs,
                FilesIDs = message.FilesIDs,
                TypeID = message.TypeID,
                Created = message.Created,
            };

            var recepientName = await _appDbContext.Users.Where(x => x.Id == getmsg.OwnerID).FirstOrDefaultAsync();
            RecipientUserView rview = new RecipientUserView();
            rview.RecipientUserId = getmsg.OwnerID;
            rview.RecipientName = new MeetupLangView() { En = recepientName.NameEn, Ar = recepientName.NameAr };
            var profileDesignation = _appDbContext.ProfileWorkExperiences.Include(k => k.Title).FirstOrDefault(k => k.ProfileId == recepientName.Id);
            if (profileDesignation != null)
            {
                rview.Designation = new MeetupLangView() { En = profileDesignation.Title?.TitleEn, Ar = profileDesignation.Title?.TitleAr };
            }
            rview.RecipientUserImageId = recepientName.SmallImageFileId;

            getmsg.OwnerInfo = rview;

            return new MessagingResponse(getmsg);
        }

        private static async Task PostImageUploadAsync(MessageAddView message, GridFSBucket bucket, Message messages)
        {
            GridFSUploadOptions options = new GridFSUploadOptions()
            {
                ChunkSizeBytes = (int)message.ImageData.Length,
                ContentType = message.ImageData.ContentType
            };
            var imgFileId =
                await bucket.UploadFromStreamAsync(message.ImageData.FileName, message.ImageData.OpenReadStream(), options);
            messages.ImagesIDs = new List<string>() { imgFileId.ToString() };
        }

        private static async Task<string> PostDocumentUploadAsync(MessageAddView message, GridFSBucket bucket, Message messages)
        {
            var documentName = message.DocumentData.FileName + "." + message.DocumentData.ContentType.Split('/')[1];
            GridFSUploadOptions docOption = new GridFSUploadOptions()
            {
                ChunkSizeBytes = (int)message.DocumentData.Length,
                ContentType = message.DocumentData.ContentType
            };
            var fileId = await bucket.UploadFromStreamAsync(documentName, message.DocumentData.OpenReadStream(), docOption);
            messages.FilesIDs = new List<string>() { fileId.ToString() };
            return documentName;
        }


        public async Task<IMessagingResponse> DeleteRoomMessageAsync(string roomId, string messageId)
        {
            try
            {
                var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();
                if (!room.Messages.Any()) return new MessagingResponse(_mapper.Map<RoomView>(room));

                room.Messages.Remove(room.Messages.FirstOrDefault(x => x.ID == new ObjectId(messageId)));
                await _mongoDbContext.Rooms.ReplaceOneAsync(x => x.ID == new ObjectId(roomId), room);
                return new MessagingResponse(_mapper.Map<RoomView>(room));
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new MessagingResponse(e);
            }
        }

        public async Task<IMessagingResponse> SetReadMessageAsync(string roomId, string messageId, int userId, bool isread)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {roomId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var room = await _mongoDbContext.Rooms.Find(x => x.ID == new ObjectId(roomId)).FirstOrDefaultAsync();
                //var message = room.Messages.FirstOrDefault(x => x.ID == new ObjectId(messageId));
                var firstName = await _appDbContext.Profiles.Where(k => k.Id == userId).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
                var lastName = await _appDbContext.Profiles.Where(k => k.Id == userId).Select(k => k.LastNameEn).FirstOrDefaultAsync();
                var userName = firstName + " " + lastName;

                    room.LastModifiedOn = DateTime.Now;
                    room.ModifiedBy = userName;

                    foreach (UnreadMessage unreadMessage in room.UnreadMessages)
                    {
                        if (unreadMessage.UserID == userId)
                        {
                            if (isread)
                            {
                                unreadMessage.MessagesCount = 0;
                            }
                            else
                            {
                                unreadMessage.MessagesCount = unreadMessage.MessagesCount + 1;
                            }
                        }
                    }
                    await _mongoDbContext.Rooms.ReplaceOneAsync(x => x.ID == new ObjectId(roomId), room);
                
                return await GetRoomMessageAsync(roomId);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new MessagingResponse(e);
            }
        }
    }
}
