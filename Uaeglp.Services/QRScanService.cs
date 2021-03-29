using AutoMapper;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Event;
using Uaeglp.ViewModels.Enums;
using System.Net;
using Uaeglp.Utilities;

namespace Uaeglp.Services
{
    public class QRScanService : IQRScanService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IEncryptionManager _Encryptor;
        private readonly IFileService _fileService;
        private readonly MongoDbContext _mongoDbContext;

        public QRScanService(MongoDbContext mongoDbContext, AppDbContext appContext, IEncryptionManager Encryption, IMapper mapper, IFileService fileService)
        {
            _appDbContext = appContext;
            _mapper = mapper;
            _Encryptor = Encryption;
            _fileService = fileService;
            _mongoDbContext = mongoDbContext;
        }
        public async Task<IQRScanResponse> ScanQRCodeAsync(int userId, string _data)
        {
            QRScanView _response = new QRScanView();
            try
            {
                //var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
                char[] spearator = { '-' };
                var Scans = _data.Split(spearator);
                if (Scans[0]== "Meetup")
                {
                    var code = Convert.ToInt32(Scans[1]);
                    var item = await _appDbContext.Meetups.FirstOrDefaultAsync(e => e.Id == code);
                    if (item == null)
                        return new QRScanResponse(ClientMessageConstant.InvalidQRCode, HttpStatusCode.NotFound);
                    else
                    {
                        
                    }

                    int profileId = userId;
                    var commentUser = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == profileId);
                    var commentUserJob = await _appDbContext.ProfileWorkExperiences.Include(m => m.Title)
                        .Where(k => k.ProfileId == profileId).OrderByDescending(k => k.DateFrom)
                        .FirstOrDefaultAsync();

                    int? smallImageFileId = commentUser.SmallImageFileId;
                    if (smallImageFileId.HasValue)
                    {
                        smallImageFileId = commentUser.SmallImageFileId;

                    }

                    _response.UserNameEN = commentUser.NameEn;
                    _response.UserNameAR = commentUser.NameAr;
                    _response.DesignationEN = commentUserJob?.Title?.TitleEn;
                    _response.DesignationAR = commentUserJob?.Title?.TitleAr;
                    _response.UserId = profileId;
                    _response.ProfileImageId = smallImageFileId;
                    _response.EventNameEN = item.Title;
                    _response.EventNameAR = "";
                    _response.LocationTextAR = "";
                    _response.LocationTextEN = "";
                    _response.Location = new MapAutocompleteView()
                    {
                        Latitude = item.Latitude,
                        Longitude = item.Longitude,
                        Text = item.MapText
                    };
                    _response.MeetupId = item.Id;
                }
                else if (Scans[0] == "Event")
                {
   
                    var code = Scans[1];
                    var item = await _appDbContext.Events.Include(a=>a.EventDays).FirstOrDefaultAsync(e => e.QRCode == code);
                    
                    if (item == null)
                        return new QRScanResponse(ClientMessageConstant.InvalidQRCode, HttpStatusCode.NotFound);
                    //else if (true)
                    //{

                    //}
                    else
                    {
                        

                        
                    }
                    int profileId = userId;
                    var commentUser = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == profileId);
                    var commentUserJob = await _appDbContext.ProfileWorkExperiences.Include(m => m.Title)
                        .Where(k => k.ProfileId == profileId).OrderByDescending(k => k.DateFrom)
                        .FirstOrDefaultAsync();

                    int? smallImageFileId = commentUser.SmallImageFileId;
                    if (smallImageFileId.HasValue)
                    {
                        smallImageFileId = commentUser.SmallImageFileId;

                    }

                    _response.UserNameEN = commentUser.NameEn;
                    _response.UserNameAR = commentUser.NameAr;
                    _response.DesignationEN = commentUserJob?.Title?.TitleEn;
                    _response.DesignationAR = commentUserJob?.Title?.TitleAr;
                    _response.UserId = profileId;
                    _response.ProfileImageId = smallImageFileId;
                    _response.EventNameEN = item.TextEn;
                    _response.EventNameAR = item.TextAr;
                    _response.LocationTextAR = item.LocationAr;
                    _response.LocationTextEN = item.LocationEn;
                    _response.Location = new MapAutocompleteView()
                    {
                        Latitude = item.Latitude,
                        Longitude = item.Longitude,
                        Text = item.MapText
                    };
                    _response.EventId = item.Id;
                }


                return new QRScanResponse(_response);
            }
            catch (Exception e)
            {

                return new QRScanResponse(e);
            }
            
        }
        public async Task<IQRScanResponse> JoinEventByQRCodeAsync(int userId,int decisionId, string _data)
        {
            QRScanView _response = new QRScanView();
            try
            {
                //var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
                char[] spearator = { '-' };
                var Scans = _data.Split(spearator);
                if (Scans[0] == "Meetup")
                {
                    var code = Convert.ToInt32(Scans[1]);
                    var item = await _appDbContext.Meetups.FirstOrDefaultAsync(e => e.Id == code);
                    if (item == null)
                        return new QRScanResponse(ClientMessageConstant.InvalidQRCode, HttpStatusCode.NotFound);
                    else
                    {
                        if (item.Date.Date < DateTime.Now.Date)
                        {
                            return new QRScanResponse(ClientMessageConstant.PastEvent, HttpStatusCode.NotFound);
                        }
                        else if (item.Date.Date > DateTime.Now.Date)
                        {
                            return new QRScanResponse(ClientMessageConstant.FutureEvent, HttpStatusCode.NotFound);
                        }

                    }
                    //throw new ArgumentException("Invalid Scan Data: " + _data);

                    if (_appDbContext.ProfileMeetups.Any(m => m.MeetupId == item.Id && m.ProfileId == userId && m.MeetupStatusItemId == 69001))
                        return new QRScanResponse(ClientMessageConstant.AlreadyJoined, HttpStatusCode.AlreadyReported);
                    //_appDbContext.ProfileMeetups.FirstOrDefault(m => m.MeetupId == item.Id && m.ProfileId == userId).MeetupStatusItemId = decisionId;
                    else
                    {

                        _appDbContext.ProfileMeetups.Add(new ProfileMeetup
                        {
                            ProfileId = userId,
                            MeetupId = item.Id,
                            MeetupStatusItemId = decisionId
                        });
                        _appDbContext.SaveChanges();
                    }

                }
                else if (Scans[0] == "Event")
                {

                    var item = await _appDbContext.Events.FirstOrDefaultAsync(e => e.QRCode == Scans[1]);
                    if (item == null)
                        return new QRScanResponse(ClientMessageConstant.InvalidQRCode, HttpStatusCode.NotFound);
                    else
                    {
                        var eventDate = await _appDbContext.EventDays.FirstOrDefaultAsync(e => e.EventId == item.Id);
                        if (eventDate != null)
                        {
                            if (eventDate.Date < DateTime.Now.Date)
                            {
                                return new QRScanResponse(ClientMessageConstant.PastEvent, HttpStatusCode.NotFound);
                            }
                            else if (eventDate.Date > DateTime.Now.Date)
                            {
                                return new QRScanResponse(ClientMessageConstant.FutureEvent, HttpStatusCode.NotFound);
                            }
                        }
                    }

                    if (_appDbContext.ProfileEvents.Any(m => m.EventId == item.Id && m.ProfileId == userId && m.EventStatusItemId == 69001))
                        return new QRScanResponse(ClientMessageConstant.AlreadyJoined, HttpStatusCode.AlreadyReported);
                    // _appDbContext.ProfileEvents.FirstOrDefault(m => m.EventId == item.Id && m.ProfileId == userId).EventStatusItemId = decisionId;
                    else
                    {
                        Application application = null;
                        int batchid = (item.BatchId.HasValue) ? item.BatchId.Value : 0;
                        if (batchid != 0)
                        {
                            application = await
                                _appDbContext.Applications.FirstOrDefaultAsync(k => k.ProfileId == userId && k.BatchId == batchid &&
                                k.StatusItemId == (int)ApplicationProgressStatus.Accepted);
                        }

                        if (item.BatchId != null && application == null)
                        {
                            return new QRScanResponse(ClientMessageConstant.NotEligible, HttpStatusCode.NotFound);
                        }

                        if (_appDbContext.ProfileEvents.Any(m => m.EventId == item.Id && m.ProfileId == userId))
                        {
                            var profileEvent = await _appDbContext.ProfileEvents.FirstOrDefaultAsync(m => m.EventId == item.Id && m.ProfileId == userId);
                            profileEvent.EventStatusItemId = decisionId;
                            _appDbContext.SaveChanges();
                        }
                        else
                        {

                            _appDbContext.ProfileEvents.Add(new ProfileEvent
                            {
                                ProfileId = userId,
                                EventId = item.Id,
                                EventStatusItemId = decisionId
                            });
                            _appDbContext.SaveChanges();
                        }
                    }

                }
                else
                {
                    return new QRScanResponse(ClientMessageConstant.InvalidQRCode, HttpStatusCode.NotFound);
                }

                return new QRScanResponse(decisionId);
            }
            catch (Exception e)
            {

                return new QRScanResponse(e);
            }
            
        }
        public async Task<IQRScanResponse> JoinAdminEventByShortCodeAsync(int eventid, string shortCode)
        {
            QRScanView _response = new QRScanView();
            try
            {
                //var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
                //char[] spearator = { '-' };
                //var Scans = qrCode.Split(spearator);
                var Scans = (shortCode != "" && shortCode != null) ? Convert.ToInt32(shortCode) : 0;
                Scans = (Scans != 0) ? Scans + 100 : Scans;

                    var item = await _appDbContext.Profiles.FirstOrDefaultAsync(e => e.Id == Scans);
                    if (item == null)
                        return new QRScanResponse(ClientMessageConstant.InvalidShortCode, HttpStatusCode.NotFound);
                    var usereventJoin = await _appDbContext.ProfileEvents.FirstOrDefaultAsync(e => e.EventId == item.Id && e.ProfileId == item.Id && e.EventStatusItemId == 69001);
                if (_appDbContext.ProfileEvents.Any(e => e.EventId == eventid && e.ProfileId == item.Id && e.EventStatusItemId == 69001))
                {
                    return new QRScanResponse(ClientMessageConstant.AlreadyJoined, HttpStatusCode.AlreadyReported);
                }
                else
                {

                    var events = await _appDbContext.Events.FirstOrDefaultAsync(e => e.Id == eventid);
                    Application application = null;
                    int batchid = (events.BatchId.HasValue) ? events.BatchId.Value : 0;
                    if (batchid != 0)
                    {


                        application = await
                            _appDbContext.Applications.FirstOrDefaultAsync(k => k.ProfileId == item.Id && k.BatchId == batchid &&
                            k.StatusItemId == (int)ApplicationProgressStatus.Accepted);
                    }

                    if (events.BatchId != null && application == null)
                    {
                        return new QRScanResponse(ClientMessageConstant.NotEligible, HttpStatusCode.NotFound);
                    }
                    _appDbContext.ProfileEvents.Add(new ProfileEvent
                    {
                        ProfileId = item.Id,
                        EventId = eventid,
                        EventStatusItemId = 69001
                    });
                    _appDbContext.SaveChanges();
                }
                

                return new QRScanResponse(69001);
            }
            catch (Exception e)
            {

                return new QRScanResponse(e);
            }
        }
        public async Task<IQRScanResponse> JoinAdminEventByQRCodeAsync(int eventid, string qrCode)
        {
            QRScanView _response = new QRScanView();
            try
            {
                //var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
                char[] spearator = { '-' };
                var Scans = qrCode.Split(spearator);

                    var item = await _appDbContext.Profiles.FirstOrDefaultAsync(e => e.QRCode == Scans[1]);
                    if (item == null)
                        return new QRScanResponse(ClientMessageConstant.InvalidQRCode, HttpStatusCode.NotFound);
                    var usereventJoin = await _appDbContext.ProfileEvents.FirstOrDefaultAsync(e => e.EventId == item.Id && e.ProfileId == item.Id && e.EventStatusItemId == 69001);
                if (_appDbContext.ProfileEvents.Any(e => e.EventId == eventid && e.ProfileId == item.Id && e.EventStatusItemId == 69001))
                {
                    return new QRScanResponse(ClientMessageConstant.AlreadyJoined, HttpStatusCode.AlreadyReported);
                    
                }
                else
                {

                    var events = await _appDbContext.Events.FirstOrDefaultAsync(e => e.Id == eventid);
                    Application application = null;
                    int batchid = (events.BatchId.HasValue) ? events.BatchId.Value : 0;
                    if (batchid != 0)
                    {


                        application = await
                            _appDbContext.Applications.FirstOrDefaultAsync(k => k.ProfileId == item.Id && k.BatchId == batchid &&
                            k.StatusItemId == (int)ApplicationProgressStatus.Accepted);
                    }

                    if (events.BatchId != null && application == null)
                    {
                        return new QRScanResponse(ClientMessageConstant.NotEligible, HttpStatusCode.NotFound);
                    }
                    _appDbContext.ProfileEvents.Add(new ProfileEvent
                    {
                        ProfileId = item.Id,
                        EventId = eventid,
                        EventStatusItemId = 69001
                    });
                    _appDbContext.SaveChanges();
                    return new QRScanResponse(69001);
                }

                

                
            }
            catch (Exception e)
            {

                return new QRScanResponse(e);
            }
            //throw new NotImplementedException();
        }

        public async Task<IQRScanResponse> GetUserDataByshortCodeAsync( string shortCode)
        {
            
            attendee at = new attendee();
            try
            {
                var Scans = (shortCode != "" && shortCode != null) ? Convert.ToInt32(shortCode) : 0;
                Scans = (Scans != 0) ? Scans + 100 : Scans;
                var item = await _appDbContext.Profiles.FirstOrDefaultAsync(e => e.Id == Scans);
                if (item == null)
                    return new QRScanResponse(ClientMessageConstant.InvalidShortCode, HttpStatusCode.NotFound);

                else
                {
                    var attendeeName = await _appDbContext.Users.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                    if (attendeeName != null)
                    {
                       
                        at.ProfileId = item.Id;
                        at.NameEn = (attendeeName.NameEn != null) ? attendeeName.NameEn : "";
                        at.NameAr = (attendeeName.NameAr != null) ? attendeeName.NameAr : "";

                        var RprofileDesignation = _appDbContext.ProfileWorkExperiences.Include(k => k.Title).FirstOrDefault(k => k.ProfileId == attendeeName.Id);
                        if (RprofileDesignation != null)
                        {
                            at.TitleEn = RprofileDesignation.Title?.TitleEn;

                            at.TitleAr = RprofileDesignation.Title?.TitleAr;
                        }
                        if (attendeeName.OriginalImageFileId != null)
                        {
                            at.ImageURL = ConstantUrlPath.ProfileImagePath + attendeeName.OriginalImageFileId;
                        }
                       
                    }
                    return new QRScanResponse(at);
                }

                
            }
            catch (Exception e)
            {

                return new QRScanResponse(e);
            }
        }
        public async Task<IQRScanResponse> GetUserDataByQrCodeAsync( string qrCode)
        {
            attendee at = new attendee();
            try
            {
                //var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
                char[] spearator = { '-' };
                var Scans = qrCode.Split(spearator);

                var item = await _appDbContext.Profiles.FirstOrDefaultAsync(e => e.QRCode == Scans[1]);
                if (item == null)
                    return new QRScanResponse(ClientMessageConstant.InvalidQRCode, HttpStatusCode.NotFound);
                else
                {
                    var attendeeName = await _appDbContext.Users.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                    if (attendeeName != null)
                    {

                        at.ProfileId = item.Id;
                        at.NameEn = (attendeeName.NameEn != null) ? attendeeName.NameEn : "";
                        at.NameAr = (attendeeName.NameAr != null) ? attendeeName.NameAr : "";

                        var RprofileDesignation = _appDbContext.ProfileWorkExperiences.Include(k => k.Title).FirstOrDefault(k => k.ProfileId == attendeeName.Id);
                        if (RprofileDesignation != null)
                        {
                            at.TitleEn = RprofileDesignation.Title?.TitleEn;

                            at.TitleAr = RprofileDesignation.Title?.TitleAr;
                        }
                        if (attendeeName.OriginalImageFileId != null)
                        {
                            at.ImageURL = ConstantUrlPath.ProfileImagePath + attendeeName.OriginalImageFileId;
                        }

                    }
                    return new QRScanResponse(at);
                }
                
            }
            catch (Exception e)
            {

                return new QRScanResponse(e);
            }
            //throw new NotImplementedException();
        }

        public async Task<IQRScanResponse> GetEvents()
        {
            try
            {
                var eveents = await _appDbContext.Events.Include(x => x.EventDays)
                           .Where(x => x.Id == x.EventDays.FirstOrDefault(a => a.Date.Date == DateTime.Now.Date).EventId && x.RegistrationTypeItemID == 98002).ToListAsync();

                return new QRScanResponse(GetEventViews(eveents,LanguageType.AR));
            }
            catch (Exception e)
            {

                return new QRScanResponse(e);
            }
        }

        public async Task<IQRScanResponse> GetEventparticipants(int eventid)
        {
            try
            {
                List<TaggedProfileView> TaggedProfiles = new List<TaggedProfileView>();
                var Users = _appDbContext.ProfileEvents.Where(x => x.EventId == eventid && x.EventStatusItemId == 69001).ToList();
                foreach (var pro in Users)
                {

                    var attendeeName = await _appDbContext.Users.Where(x => x.Id == pro.ProfileId).FirstOrDefaultAsync();

                    if (attendeeName != null)
                    {
                        TaggedProfileView at = new TaggedProfileView();
                        at.ProfileId = pro.ProfileId;
                        at.NameEn = (attendeeName.NameEn != null) ? attendeeName.NameEn : "";
                        at.NameAr = (attendeeName.NameAr != null) ? attendeeName.NameAr : "";

                        var RprofileDesignation = _appDbContext.ProfileWorkExperiences.Include(k => k.Title).FirstOrDefault(k => k.ProfileId == attendeeName.Id);
                        if (RprofileDesignation != null)
                        {
                            at.TitleEn = RprofileDesignation.Title?.TitleEn;

                            at.TitleAr = RprofileDesignation.Title?.TitleAr;
                        }
                        if (attendeeName.OriginalImageFileId != null)
                        {
                            at.ImageURL = ConstantUrlPath.ProfileImagePath + attendeeName.OriginalImageFileId;
                        }
                        at.EventCount = _appDbContext.ProfileEvents.Count(a => a.ProfileId == pro.ProfileId && a.EventStatusItemId == 69001);
                        TaggedProfiles.Add(at);
                    }
                }
                return new QRScanResponse(TaggedProfiles);
            }
            catch (Exception e)
            {

                return new QRScanResponse(e);
            }
        }

        private List<AdminRegisterEventView> GetEventViews(List<Event> events, LanguageType language = LanguageType.AR)
        {
            var eventViews = new List<AdminRegisterEventView>();
            foreach (var e in events)
            {
                if (e != null)
                {
                    var eventView = new AdminRegisterEventView
                    {
                        ID = e.Id,
                        LocationEN = e.LocationEn,
                        LocationAR = e.LocationAr,
                        TextEN = e.TextEn,
                        TextAR = e.TextAr,
                        DescriptionAR = e.DescriptionAr,
                        DescriptionEN = e.DescriptionEn,
                        
                        MapAutocompleteView = new MapAutocompleteView()
                        {
                            Latitude = e.Latitude,
                            Longitude = e.Longitude,
                            Text = e.MapText
                        }
                    };

                    eventViews.Add(eventView);
                }
            }
            return eventViews;
        }
    }
}
