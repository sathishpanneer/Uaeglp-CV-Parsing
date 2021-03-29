using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.ViewModels;
using Uaeglp.Models;
using Microsoft.AspNetCore.Http;
using Uaeglp.Utilities;
using File = Uaeglp.Models.File;
using Microsoft.Extensions.Options;
using System.IO;
using System.Linq;
using Uaeglp.ViewModels.Enums;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Uaeglp.ViewModels.ProfileViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using NLog;
using Profile = Uaeglp.Models.Profile;
using Minio;
using Minio.Exceptions;

namespace Uaeglp.Services
{
    public class RecommendLeaderService : IRecommendLeaderService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly AppDbContext _appDbContext;
        private readonly MinIoConfig _minIoConfig;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IEncryptionManager _encryptor;
        private readonly IUserIPAddress _userIPAddress;

        public RecommendLeaderService(AppDbContext appDbContext, IOptions<MinIoConfig> minIoConfig, IMapper mapper, IEmailService emailService, IEncryptionManager encryption, IUserIPAddress userIPAddress)
        {
            _appDbContext = appDbContext;
            _minIoConfig = minIoConfig.Value;
            _mapper = mapper;
            _emailService = emailService;
            _encryptor = encryption;
            _userIPAddress = userIPAddress;
        }

        public async Task<IRecommendLeaderResponse> AddRecommendLeader(RecommendLeaderView view)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {view.ToJsonString()} UserIPAddress: { _userIPAddress.GetUserIP().Result }");


                var firstName = await _appDbContext.Profiles.Where(k => k.Id == view.ProfileID).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
                var lastName = await _appDbContext.Profiles.Where(k => k.Id == view.ProfileID).Select(k => k.LastNameEn).FirstOrDefaultAsync();
                var userName = firstName + " " + lastName;

                var matchingProfileId = 0;
                var profileUrl = "";

                var RecommendStatusItem = await _appDbContext.LookupItems.Where(k => k.LookupId == (int)LookupType.StatusItem).OrderBy(k => k.NameEn).FirstOrDefaultAsync();

                //var email = await _appDbContext.UserInfos.Where(k => k.UserId == view.ProfileID).Select(k => k.Email).FirstOrDefaultAsync();
                var linkedinUrl = new Profile();
                var TwitterUrl = new Profile();
                var email = new UserInfo();
                var mobile = new UserInfo();
                if (view.LinkedinURL != null)
                {
                    linkedinUrl = await _appDbContext.Profiles.Where(x => x.LinkedInUrl == view.LinkedinURL).FirstOrDefaultAsync();
                }
                if(view.TwitterURL != null)
                {
                    TwitterUrl = await _appDbContext.Profiles.Where(x => x.TwitterUrl == view.TwitterURL).FirstOrDefaultAsync();
                }

                if (view.Email != null)
                {
                    email = await _appDbContext.UserInfos.Where(x => x.Email == view.Email).FirstOrDefaultAsync();
                }

                if (view.ContactNumber != null)
                {
                    mobile = await _appDbContext.UserInfos.Where(x => x.Mobile == view.ContactNumber).FirstOrDefaultAsync();
                }

                if (linkedinUrl?.LinkedInUrl != null || TwitterUrl?.TwitterUrl != null || email?.Email != null || mobile?.Mobile != null)
                {
                    profileUrl = "https://stagingplatform.uaeglp.gov.ae/";
                    matchingProfileId = email.Email != null ? email.Id 
                                        : mobile.Mobile != null ? mobile.Id
                                        : linkedinUrl.LinkedInUrl != null ? linkedinUrl.Id
                                        : TwitterUrl.TwitterUrl != null ? TwitterUrl.Id : 0;
                }

                var recommendLeader = new RecommendLeadr()
                {
                    FullName = view.FullName,
                    RecommendingText = view.RecommendingText,
                    Occupation = view.Occupation,
                    ContactNumber = view.ContactNumber,
                    Email = view.Email,
                    LinkedinURL = view.LinkedinURL,
                    TwitterURL = view.TwitterURL,
                    InstagramURL = view.InstagramURL,
                    RecommenderProfileID = view.ProfileID != 0 ? view.ProfileID : null,
                    OtherFitment = view.OthersText,
                    SourceItemID = view.SourceItemID,
                    StatusItemID = RecommendStatusItem.Id,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = view.ProfileID != null ? userName : "AnonymousUser",
                    ModifiedBy = view.ProfileID != null ? userName : "AnonymousUser",
                    RecommendedProfileID = matchingProfileId != 0 ? matchingProfileId : (int?)null
                };

                if (view.AudioFile != null)
                {
                    var audioGuid = Guid.NewGuid();
                    recommendLeader.RecommendingAudioID = audioGuid;
                    minioAudioVideoUpload(view.AudioFile, "audio", audioGuid);
                }

                if (view.VideoFile != null)
                {
                    var videoGuid = Guid.NewGuid();
                    recommendLeader.RecommendingVideoID = videoGuid;
                    minioAudioVideoUpload(view.VideoFile, "video", videoGuid);
                }

                await _appDbContext.RecommandLeaders.AddAsync(recommendLeader);
                await _appDbContext.SaveChangesAsync();

                var recommendId = await _appDbContext.RecommandLeaders.Where(x => x.ID == recommendLeader.ID).Select(x => x.ID).FirstOrDefaultAsync();

                List<int?> FitList = new List<int?>();

                FitList = view.RecommendLeaderFit != null ? view.RecommendLeaderFit : null;

                var recommendFit = new RecommendationFitDetails();
                var recommendFitOther = new RecommandationOther();
                if (FitList != null)
                {
                    foreach (var item in FitList)
                    {
                        recommendFit = new RecommendationFitDetails()
                        {
                            RecommendID = recommendId,
                            RecommendationFitItemId = item
                        };

                        await _appDbContext.RecommendationFitDetails.AddAsync(recommendFit);
                        await _appDbContext.SaveChangesAsync();
                        _appDbContext.Entry(recommendFit).State = EntityState.Detached;
                    }
                }

                var data = new RecommendLeaderSubmissionView()
                {
                    RecommendLeaderSubmission = _mapper.Map<RecommendSubmissionView> (recommendLeader),
                    RecommendLeaderFitment = FitList,
                };

                data.RecommendLeaderSubmission.RecommendViewProfileURL = profileUrl;
                return new RecommendLeaderResponse(data);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }

        }

        public  bool minioAudioVideoUpload(IFormFile formFile, string fileType, Guid guid)
        {
            var appSetting = new MinIoConfig()
            {
                EndPoint = _minIoConfig.EndPoint,
                AccessKey = _minIoConfig.AccessKey,
                SecretKey = _minIoConfig.SecretKey,
                BucketName = _minIoConfig.BucketName,
                Location = _minIoConfig.Location,
                MinIoForDev = _minIoConfig.MinIoForDev,
                FilePath = _minIoConfig.FilePath

            };

            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  EndPoint : {appSetting.EndPoint}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  AccessKey : {appSetting.AccessKey}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SecretKey : {appSetting.SecretKey}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  BucketName : {appSetting.BucketName}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  Location : {appSetting.Location}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  Location : {appSetting.MinIoForDev}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  UserIPAddress: { _userIPAddress.GetUserIP().Result }");
            try
            {
                if(appSetting.MinIoForDev != true)
                {
                    var minio = new MinioClient(appSetting.EndPoint, appSetting.AccessKey, appSetting.SecretKey).WithSSL();
                    Run(minio, formFile, appSetting.BucketName, appSetting.Location, guid, fileType, appSetting.FilePath).Wait();
                    return true;
                }
                else
                {
                    var minio = new MinioClient(appSetting.EndPoint, appSetting.AccessKey, appSetting.SecretKey);
                    Run(minio, formFile, appSetting.BucketName, appSetting.Location, guid, fileType, appSetting.FilePath).Wait();
                    return true;
                } 
                
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }
        private async static Task Run(MinioClient minio, IFormFile _request, string bucketName, string location,  Guid guid, string fileType, string fileLocation)
        {
            if (!Directory.Exists(fileLocation))
            {
                Directory.CreateDirectory(fileLocation);
            }

            string FilePath = "";

            

            using (var fileStream = new FileStream(fileLocation + _request.FileName, FileMode.Create))
            {
                await _request.CopyToAsync(fileStream);
                FilePath = fileStream.Name;
            }

            var objectName = "";
            if (fileType == "audio")
            {
                //var fileExtension = _request.FileName.Split(".");
                objectName = guid.ToString();

            }
            else if (fileType == "video")
            { 
                objectName = guid.ToString();
            }


            var filePath = FilePath;

            var contentType = "";
            if(fileType == "audio")
            {
                contentType = _request.ContentType;
            }
            else if(fileType == "video")
            {
                contentType = _request.ContentType;
            }

            try
            {
                // Make a bucket on the server, if not already present.
                bool found = await minio.BucketExistsAsync(bucketName);
                if (!found)
                {
                    await minio.MakeBucketAsync(bucketName, location);
                }
                // Upload a file to bucket.
                await minio.PutObjectAsync(bucketName, objectName, filePath, contentType);
                System.IO.File.Delete(filePath);
                Console.WriteLine("Successfully uploaded " + objectName);

            }
            catch (MinioException e)
            {
                logger.Error(e);
                Console.WriteLine("File Upload Error: {0}", e.Message);
            }

        }
        public async Task<IRecommendLeaderResponse> GetRecommendFitListAsync()
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() } UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                var RecommendFitList = await _appDbContext.LookupItems.Where(k => k.LookupId == (int)LookupType.Recommendation).OrderBy(k => k.NameEn).ToListAsync();
                var RecommendSourceItemList = await _appDbContext.LookupItems.Where(k => k.LookupId == (int)LookupType.SourceItem).OrderBy(k => k.NameEn).ToListAsync();
                var RecommendStatusItem = await _appDbContext.LookupItems.Where(k => k.LookupId == (int)LookupType.StatusItem).OrderBy(k => k.NameEn).ToListAsync();

                var data = new RecommendationFitDetailsView()
                {
                    RecommendLeaderFitList = _mapper.Map<List<LookupItemView>>(RecommendFitList),
                    RecommendLeaderSourceItemList = _mapper.Map<List<LookupItemView>>(RecommendSourceItemList),
                    RecommendLeaderStatusItem = _mapper.Map<List<LookupItemView>>(RecommendStatusItem)
                };

                return new RecommendLeaderResponse(data);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }

        }


        public async Task<IRecommendLeaderResponse> getRecommendLeaderDetailsAsync(int recommendId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {recommendId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                var recommendSubmission = await _appDbContext.RecommandLeaders.Where(x => x.ID == recommendId).FirstOrDefaultAsync();
                var recommendLeader = new RecommendLeadr()
                {
                    ID = recommendSubmission.ID,
                    FullName = recommendSubmission.FullName,
                    RecommendingText = recommendSubmission.RecommendingText,
                    Occupation = recommendSubmission.Occupation,
                    ContactNumber = recommendSubmission.ContactNumber,
                    Email = recommendSubmission.Email,
                    LinkedinURL = recommendSubmission.LinkedinURL,
                    TwitterURL = recommendSubmission.TwitterURL,
                    InstagramURL = recommendSubmission.InstagramURL,
                    RecommenderProfileID = recommendSubmission.RecommenderProfileID,
                    RecommendedProfileID = recommendSubmission.RecommendedProfileID,
                    RecommendingAudioID = recommendSubmission.RecommendingAudioID,
                    RecommendingVideoID = recommendSubmission.RecommendingVideoID,
                    OtherFitment = recommendSubmission.OtherFitment,
                    SourceItemID = recommendSubmission.SourceItemID,
                    StatusItemID = recommendSubmission.StatusItemID,
                    Created = recommendSubmission.Created,
                    Modified = recommendSubmission.Modified,
                    CreatedBy = recommendSubmission.CreatedBy,
                    ModifiedBy = recommendSubmission.ModifiedBy
                };

                var recommendFit = await _appDbContext.RecommendationFitDetails.Where(x => x.RecommendID == recommendId).Select(x => x.RecommendationFitItemId).ToListAsync();
                List<RecommendFitListView> fitmentList = new List<RecommendFitListView>();
                foreach (var item in recommendFit)
                {
                    if (item != null)
                    {
                        var RecommendFitList = await _appDbContext.LookupItems.Where(k => k.Id == item).OrderBy(k => k.NameEn).FirstOrDefaultAsync();

                        RecommendFitListView fitList = new RecommendFitListView()
                        {
                            ID = RecommendFitList.Id,
                            NameEn = RecommendFitList.NameEn,
                            NameAr = RecommendFitList.NameAr
                        };
                        fitmentList.Add(fitList);
                    }
                }

                var data = new RecommendLeaderSubmissionView()
                {
                    RecommendLeaderSubmission = _mapper.Map<RecommendSubmissionView>(recommendLeader),
                    RecommendLeaderFitmentDetails = fitmentList,
                };
                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == recommendSubmission.RecommenderProfileID);
                var workExperience = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title)
                    .Where(k => k.ProfileId == recommendSubmission.RecommenderProfileID).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();
                var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == recommendSubmission.RecommenderProfileID);
                if (profile != null)
                {
                    data.RecommendLeaderSubmission.RecommendedBy = new PublicProfileView()
                    {
                        Id = profile.Id,
                        FirstNameAr = profile.FirstNameAr,
                        FirstNameEn = profile.FirstNameEn,
                        LastNameAr = profile.LastNameAr,
                        LastNameEn = profile.LastNameEn,
                        SecondNameAr = profile.SecondNameAr,
                        SecondNameEn = profile.SecondNameEn,
                        ThirdNameAr = profile.ThirdNameAr,
                        ThirdNameEn = profile.ThirdNameEn,
                        Designation = workExperience?.Title?.TitleEn,
                        DesignationAr = workExperience?.Title?.TitleAr,
                        UserImageFileId = user?.OriginalImageFileId ?? 0
                    };
                }

                return new RecommendLeaderResponse(data);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public async Task<IRecommendLeaderResponse> RequestCallbackAsync(RecommendationCallbackView view)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {view.ToJsonString()} UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                var profileId = _appDbContext.RecommandLeaders.Where(x => x.ID == view.RecommendID).Select(x => x.RecommenderProfileID).FirstOrDefault();
                var email = await _appDbContext.UserInfos.Where(k => k.UserId == profileId).Select(k => k.Email).FirstOrDefaultAsync();
                var firstName = await _appDbContext.Profiles.Where(k => k.Id == profileId).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
                var lastName = await _appDbContext.Profiles.Where(k => k.Id == profileId).Select(k => k.LastNameEn).FirstOrDefaultAsync();
                var userName = firstName + " " + lastName;
                var data = new RecommendationCallback()
                {
                    RecommendID = view.RecommendID,
                    FullName = view.FullName,
                    ContactNumber = view.ContactNumber,
                    Email = view.Email,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = profileId != null ? userName : "AnonymousUser",
                    ModifiedBy = profileId != null ? userName : "AnonymousUser",

                };

                await _appDbContext.RecommendationCallback.AddAsync(data);
                await _appDbContext.SaveChangesAsync();


                await _emailService.SendRequestCallbackEmailAsync(view.FullName, view.Email, view.ContactNumber, email, userName);


                var requestcallback = _mapper.Map<RecommendationCallbackView>(data);
                return new RecommendLeaderResponse(requestcallback);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }



        }

        public async Task<RecommendProfileView> GetViewMatchProfile(int recommendId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {recommendId} UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                var profileUrl = "";

                var profileRecommended = await _appDbContext.RecommandLeaders.Where(x => x.RecommendedProfileID == 0 || x.RecommendedProfileID == null).ToListAsync();

                foreach (var item in profileRecommended)
                {
                    var linkedinUrl = new Profile();
                    var TwitterUrl = new Profile();
                    var email = new UserInfo();
                    var mobile = new UserInfo();
                    if (item.LinkedinURL != null)
                    {
                        linkedinUrl = await _appDbContext.Profiles.Where(x => x.LinkedInUrl == item.LinkedinURL).FirstOrDefaultAsync();
                    }
                    if (item.TwitterURL != null)
                    {
                        TwitterUrl = await _appDbContext.Profiles.Where(x => x.TwitterUrl == item.TwitterURL).FirstOrDefaultAsync();
                    }

                    if (item.Email != null)
                    {
                        email = await _appDbContext.UserInfos.Where(x => x.Email == item.Email).FirstOrDefaultAsync();
                    }

                    if (item.TwitterURL != null)
                    {
                        mobile = await _appDbContext.UserInfos.Where(x => x.Mobile == item.ContactNumber).FirstOrDefaultAsync();
                    }

                    if ((item.LinkedinURL != null && linkedinUrl?.LinkedInUrl != null) || (item.TwitterURL != null && TwitterUrl?.TwitterUrl != null) || (item.Email != null && email?.Email != null) || (item.ContactNumber != null && mobile?.Mobile != null))
                    {
                        item.RecommendedProfileID = email.Email != null ? email.Id
                                                    : mobile.Mobile != null ? mobile.Id
                                                    : linkedinUrl.LinkedInUrl != null ? linkedinUrl.Id
                                                    : TwitterUrl.TwitterUrl != null ? TwitterUrl.Id : 0;

                        await _appDbContext.SaveChangesAsync();
                    }

                    

                }

                var data = new RecommendProfileView();
                
                var recommendUser = await _appDbContext.RecommandLeaders.Where(x => x.ID == recommendId).FirstOrDefaultAsync();

                var linkedinUrlVal = new Profile();
                var TwitterUrlVal = new Profile();
                var emailVal = new UserInfo();
                var mobileVal = new UserInfo();

                if (recommendUser.LinkedinURL != null)
                {
                    linkedinUrlVal = await _appDbContext.Profiles.Where(x => x.LinkedInUrl == recommendUser.LinkedinURL).FirstOrDefaultAsync();
                }
                if (recommendUser.TwitterURL != null)
                {
                    TwitterUrlVal = await _appDbContext.Profiles.Where(x => x.TwitterUrl == recommendUser.TwitterURL).FirstOrDefaultAsync();
                }

                if (recommendUser.Email != null)
                {
                    emailVal = await _appDbContext.UserInfos.Where(x => x.Email == recommendUser.Email).FirstOrDefaultAsync();
                }

                if (recommendUser.ContactNumber != null)
                {
                    mobileVal = await _appDbContext.UserInfos.Where(x => x.Mobile == recommendUser.ContactNumber).FirstOrDefaultAsync();
                }

                if (linkedinUrlVal?.LinkedInUrl != null || TwitterUrlVal?.TwitterUrl != null || emailVal?.Email != null || mobileVal?.Mobile != null)
                {
                    profileUrl = "https://stagingplatform.uaeglp.gov.ae/";
                    
                }

                var fullname = recommendUser.FullName != null ? recommendUser.FullName.Trim().Split(new char[0]) : null;
                List<Profile> matchProfiles = new List<Profile>();
                if (fullname != null) {
                    if (fullname.Length == 1)
                    {
                        matchProfiles = await _appDbContext.Profiles.Where(x => x.FirstNameEn.StartsWith(fullname[0]) || x.LastNameEn.StartsWith(fullname[0])).ToListAsync();
                    }
                    else if(fullname.Length == 2)
                    {
                        matchProfiles = await _appDbContext.Profiles.Where(x => x.FirstNameEn.StartsWith(fullname[0]) || x.FirstNameEn.StartsWith(fullname[1]) ||
                                                                                x.LastNameEn.StartsWith(fullname[0]) || x.LastNameEn.StartsWith(fullname[1])).ToListAsync();
                    }
                    else if(fullname.Length == 3)
                    {
                        matchProfiles = await _appDbContext.Profiles.Where(x => x.FirstNameEn.StartsWith(fullname[0]) || x.FirstNameEn.StartsWith(fullname[1]) || x.FirstNameEn.StartsWith(fullname[2]) ||
                                                                                x.LastNameEn.StartsWith(fullname[0])  || x.LastNameEn.StartsWith(fullname[1]) || x.LastNameEn.StartsWith(fullname[2])).ToListAsync();

                    }
                    else if (fullname.Length == 4)
                    {
                        matchProfiles = await _appDbContext.Profiles.Where(x => x.FirstNameEn.StartsWith(fullname[0]) || x.FirstNameEn.StartsWith(fullname[1]) || x.FirstNameEn.StartsWith(fullname[2]) || x.FirstNameEn.StartsWith(fullname[3]) ||
                                                                                x.LastNameEn.StartsWith(fullname[0]) || x.LastNameEn.StartsWith(fullname[1]) || x.LastNameEn.StartsWith(fullname[2]) || x.LastNameEn.StartsWith(fullname[3])).ToListAsync();

                    }
                }
                //var matchProfile = await _appDbContext.Profiles.Where(x => recommendUser.FullName.Contains(x.FirstNameEn) ||
                //recommendUser.FullName.Contains(x.LastNameEn)).ToListAsync();
                //var matchProfile = await _appDbContext.Profiles.FromSqlRaw($"SELECT * FROM dbo.Profile where soundex(firstNameEn) = soundex('{recommendUser.FullName}')").ToListAsync();
                //var matchProfile = await _appDbContext.Profiles.FromSqlRaw($"select  * from dbo.Profile where difference(soundex(FirstNameEN + ' ' + LastNameEN),soundex('{recommendUser.FullName}')) = 4").ToListAsync();

                List<PublicProfileView> matchProfileDetails = new List<PublicProfileView>();
                foreach (var item in matchProfiles)
                {
                    var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == item.Id);
                    var workExperience = await _appDbContext.ProfileWorkExperiences.Include(k => k.Title)
                        .Where(k => k.ProfileId == item.Id).OrderByDescending(y => y.DateFrom).FirstOrDefaultAsync();
                    var userDetails = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == item.Id);

                    PublicProfileView user = new PublicProfileView()
                    {
                        Id = profile.Id,
                        FirstNameAr = profile.FirstNameAr,
                        FirstNameEn = profile.FirstNameEn,
                        LastNameAr = profile.LastNameAr,
                        LastNameEn = profile.LastNameEn,
                        SecondNameAr = profile.SecondNameAr,
                        SecondNameEn = profile.SecondNameEn,
                        ThirdNameAr = profile.ThirdNameAr,
                        ThirdNameEn = profile.ThirdNameEn,
                        Designation = workExperience?.Title?.TitleEn,
                        DesignationAr = workExperience?.Title?.TitleAr,
                        UserImageFileId = userDetails.OriginalImageFileId ?? 0
                    };

                    matchProfileDetails.Add(user);
                }

                if (matchProfiles != null)
                {
                    data = new RecommendProfileView()
                    {
                        RecommendMatchProfileList = matchProfileDetails,
                        RecommendViewProfileURL = profileUrl
                    };
                }

                return data;
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        public async Task<List<RecommendSubmissionView>> GetAllRecommendLeaderListAsync(int skip, int limit)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() } UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                var recommendSubmission = await _appDbContext.RecommandLeaders.OrderByDescending(x => x.Created).Skip(skip).Take(limit).ToListAsync();
                //var profileIds = recommendSubmission
                ///var  profileUrl = "";
                //int profileId = 0;
                var linkedinUrl = new Profile();
                var TwitterUrl = new Profile();
                var email = new UserInfo();
                var mobile = new UserInfo();

                var recommendLeaderList = _mapper.Map<List<RecommendSubmissionView>>(recommendSubmission);

                foreach (var item in recommendLeaderList)
                {
                    var profileUrl = "";
                    var stagingUrl = "https://stagingplatform.uaeglp.gov.ae/";
                    //int profileId = 0;
                    var recommendUser = await _appDbContext.RecommandLeaders.Where(x => x.ID == item.ID).FirstOrDefaultAsync();

                    if (recommendUser.LinkedinURL != null)
                    {
                        linkedinUrl = await _appDbContext.Profiles.Where(x => x.LinkedInUrl == recommendUser.LinkedinURL).FirstOrDefaultAsync();
                    }
                    if (recommendUser.TwitterURL != null)
                    {
                        TwitterUrl = await _appDbContext.Profiles.Where(x => x.TwitterUrl == recommendUser.TwitterURL).FirstOrDefaultAsync();
                    }

                    if (recommendUser.Email != null)
                    {
                        email = await _appDbContext.UserInfos.Where(x => x.Email == recommendUser.Email).FirstOrDefaultAsync();
                    }

                    if (recommendUser.ContactNumber != null)
                    {
                        mobile = await _appDbContext.UserInfos.Where(x => x.Mobile == recommendUser.ContactNumber).FirstOrDefaultAsync();
                    }

                    if ((recommendUser.LinkedinURL != null && linkedinUrl?.LinkedInUrl != null) || (recommendUser.TwitterURL != null && TwitterUrl?.TwitterUrl != null) || (recommendUser.Email != null && email?.Email != null) || (recommendUser.ContactNumber != null && mobile?.Mobile != null))
                    { 
                        profileUrl = email.Email != null ? stagingUrl
                                                    : mobile.Mobile != null ? stagingUrl
                                                    : linkedinUrl.LinkedInUrl != null ? stagingUrl
                                                    : TwitterUrl.TwitterUrl != null ? stagingUrl : "";
                    }
                    item.RecommendViewProfileURL = profileUrl;
                    item.RecommendViewProfileID = recommendUser.RecommendedProfileID;
                }

                return recommendLeaderList;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }


    }
}
