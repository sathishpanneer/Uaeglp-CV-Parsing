using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;
using NLog;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.Utilities;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;
using File = Uaeglp.Models.File;

namespace Uaeglp.Services
{
    public class ProfileAssessmentService : IProfileAssessmentService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly AppDbContext _appDbContext;
        private readonly FileDbContext _fileDbContext;
        private readonly IProfilePercentageCalculationService _completePercentageService;
        private readonly IMapper _mapper;
        private readonly IYoutubeVideoUploadService _youtubeVideoUploadService;
        private readonly IEncryptionManager _encryption;
        private readonly MinIoConfig _minIoConfig;
        private readonly IUserIPAddress _userIPAddress;

        public ProfileAssessmentService(Repositories.AppDbContext appDbContext, IMapper mapper, FileDbContext fileDbContext, IProfilePercentageCalculationService completePercentageService, IYoutubeVideoUploadService youtubeVideoUploadService, IEncryptionManager encryption, IOptions<MinIoConfig> minIoConfig, IUserIPAddress userIPAddress)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _fileDbContext = fileDbContext;
            _completePercentageService = completePercentageService;
            _youtubeVideoUploadService = youtubeVideoUploadService;
            _encryption = encryption;
            _minIoConfig = minIoConfig.Value;
            _userIPAddress = userIPAddress;
        }


        public async Task<IProfileAssessmentResponse> GetBioInfoAsync(int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var profile = await _appDbContext.Profiles.Include(k => k.ResidenceCountry).FirstOrDefaultAsync(k => k.Id == userId);
                if (profile == null)
                {
                    return new ProfileAssessmentResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                var data = new BioView()
                {
                    ExpressYourSelf = profile.ExpressYourself,
                    ExpressYourSelfURL = string.IsNullOrWhiteSpace(profile.ExpressYourselfUrl) ? "" : $"/api/File/get-download-video/{profile.Id}"
                };
                return new ProfileAssessmentResponse(data);
            }
            catch (Exception e)
            {
                return new ProfileAssessmentResponse(e);
            }


        }

        public async Task<IProfileAssessmentResponse> UpdateBioInfoAsync(BioVideoView model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var profile = await _appDbContext.Profiles.Include(k => k.ResidenceCountry).FirstOrDefaultAsync(k => k.Id == model.UserId).ConfigureAwait(false);
                if (profile == null)
                {
                    return new ProfileAssessmentResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                if (model.ExpressYourSelfVideo != null)
                {
                    //_youtubeVideoUploadService.Upload("video", "profileVideo", PrivacyStatus.Unlisted, "",
                    // _encryption.Encrypt(model.UserId.ToString()), model.ExpressYourSelfVideo);

                    var fileId = await UploadBioVideoAsync(new UploadBioVideoView()
                    { File = model.ExpressYourSelfVideo, UserId = model.UserId }).ConfigureAwait(false);
                    profile.ExpressYourselfUrl = fileId.ToString();
                }
                else if (model.IsDeleted)
                {
                    profile.ExpressYourselfUrl = null;
                }

                profile.ExpressYourself = model.ExpressYourSelf;

                await _appDbContext.SaveChangesAsync().ConfigureAwait(false);


                var data = new BioView()
                {
                    UserId = profile.Id,
                    ExpressYourSelf = profile.ExpressYourself,
                    ExpressYourSelfURL = "",
                    ProfileCompletedPercentage = profile.CompletenessPercentage
                };

                if (!string.IsNullOrWhiteSpace(profile.ExpressYourselfUrl))
                {
                    data.ExpressYourSelfURL = $@"/api/File/get-download-video/{profile.Id}";
                }

                return new ProfileAssessmentResponse(data);
            }
            catch (Exception e)
            {
                return new ProfileAssessmentResponse(e);
            }


        }

        private async Task<int> UploadBioVideoAsync(UploadBioVideoView model)
        {

            try
            {

                var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.Id == model.UserId).ConfigureAwait(false);
                var data = new File()
                {
                    IdGuid = Guid.NewGuid(),
                    SizeMb = GetFileSize(model.File.Length),
                    Name = model.File.FileName,
                    ProviderName = "SqlProvider",
                    ExtraParams = model.ExtraParams,
                    Created = DateTime.Now,
                    MimeType = model.File.ContentType,
                    Modified = DateTime.Now,
                    CreatedBy = userInfo.Email,
                    ModifiedBy = userInfo.Email
                };

                var savedEntity = (await _appDbContext.Files.AddAsync(data)).Entity;

                await _appDbContext.SaveChangesAsync().ConfigureAwait(false);

                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == model.UserId).ConfigureAwait(false);

                profile.ExpressYourselfUrl = savedEntity.Id.ToString();

                await _appDbContext.SaveChangesAsync().ConfigureAwait(false);

                minioAudioVideoUpload(model.File, savedEntity.IdGuid);

                //var fileDb = new FileDB()
                //{
                //    Id = savedEntity.IdGuid,
                //    Bytes = StreamToBytes(model.File.OpenReadStream())
                //};

                //await _fileDbContext.FileDB.AddAsync(fileDb);
                //await _fileDbContext.SaveChangesAsync().ConfigureAwait(false);

                return savedEntity.Id;
            }
            catch (Exception e)
            {
                return 0;
            }

        }

        public bool minioAudioVideoUpload(IFormFile formFile, Guid id)
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
            try
            {
                if (appSetting.MinIoForDev != true)
                {
                    var minio = new MinioClient(appSetting.EndPoint, appSetting.AccessKey, appSetting.SecretKey).WithSSL();
                    Run(minio, formFile, appSetting.BucketName, appSetting.Location, id, appSetting.FilePath).Wait();
                    return true;
                }
                else
                {
                    var minio = new MinioClient(appSetting.EndPoint, appSetting.AccessKey, appSetting.SecretKey);
                    Run(minio, formFile, appSetting.BucketName, appSetting.Location, id, appSetting.FilePath).Wait();
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }
        private async static Task Run(MinioClient minio, IFormFile _request, string bucketName, string location, Guid id, string fileLocation)
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

            //var objectName = _request.FileName;
            var objectName = id.ToString();
            var filePath = FilePath;

            var contentType = _request.ContentType;

            //var contentType = "";
            //if (fileType == "audio")
            //{
            //    contentType = "audio/mp3";
            //}
            //else if (fileType == "video")
            //{
            //    contentType = "video/mp4";
            //}

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

        public async Task<IProfileAssessmentResponse> AddOrUpdateProfileEducationAsync(ProfileEducationView model)
        {

            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.UserId == model.ProfileId).ConfigureAwait(false);
                if (userInfo == null)
                {
                    return new ProfileAssessmentResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                var org = await UpdateGlpOrganizationAsync(model.OrganizationId, model.OrganizationName, userInfo).ConfigureAwait(false);

                var fieldOfStudy = !string.IsNullOrWhiteSpace(model.FieldOfStudyString) ? await FieldOfStudyAsync(model, userInfo).ConfigureAwait(false) : null;

                var isAdd = false;
                var profileEducation = await _appDbContext.ProfileEducations.Include(k => k.Profile)
                    .FirstOrDefaultAsync(k => k.Id == model.Id).ConfigureAwait(false);

                if (profileEducation == null)
                {
                    profileEducation = new ProfileEducation()
                    {
                        ProfileId = model.ProfileId,
                        Created = DateTime.Now
                    };
                    isAdd = true;
                }

                profileEducation.Finshed = model.IsStudied;
                profileEducation.CountryId = model.CountryId;
                profileEducation.Modified = DateTime.Now;
                profileEducation.DegreeItemId = model.Title?.Equals("School", StringComparison.InvariantCultureIgnoreCase) ?? false ? 56005 : model.DegreeLookupItemId; // TODO : Not Sure the mapping is correct
                profileEducation.OrganizationId = org.Id;
                profileEducation.FieldOfStudy = fieldOfStudy?.TitleEn ?? "";
                profileEducation.Title = model.Title?.Equals("School", StringComparison.InvariantCultureIgnoreCase) ?? false ? nameof(model.Title).ToLower() : fieldOfStudy?.TitleEn; // TODO : Not Sure the mapping is correct
                profileEducation.Year = model.Year;
                profileEducation.FieldOfStudyId = fieldOfStudy?.Id;
                profileEducation.CreatedBy = userInfo.Email;
                profileEducation.ModifiedBy = userInfo.Email;
                profileEducation.EmirateItemId = model.EmirateItemId;

                if (isAdd)
                {
                    await _appDbContext.ProfileEducations.AddAsync(profileEducation);
                }

                await _appDbContext.SaveChangesAsync().ConfigureAwait(false);

                var education = await _appDbContext.ProfileEducations
                    .Include(k => k.Country)
                    .Include(k => k.DegreeItem)
                    .Include(k => k.FieldOfStudyNavigation)
                    .Include(k => k.EmirateItem)
                    .Include(k => k.Organization)
                    .FirstOrDefaultAsync(k => k.Id == profileEducation.Id).ConfigureAwait(false);

                var data = _mapper.Map<ProfileEducationView>(education);

                data.ProfileCompletedPercentage = await _completePercentageService.UpdateProfileCompletedPercentageAsync(model.ProfileId);

                return new ProfileAssessmentResponse(data);
            }
            catch (Exception e)
            {
                return new ProfileAssessmentResponse(e);
            }

        }

        public async Task<IProfileAssessmentResponse> DeleteProfileEducationAsync(int profileEducationId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {profileEducationId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var data = new ProfileEducationView();
                var profileEducation =
                    await _appDbContext.ProfileEducations.FirstOrDefaultAsync(k => k.Id == profileEducationId);
                if (profileEducation != null)
                {

                    _appDbContext.ProfileEducations.Remove(profileEducation);
                    await _appDbContext.SaveChangesAsync();

                    data.ProfileCompletedPercentage = await _completePercentageService.UpdateProfileCompletedPercentageAsync(profileEducation.ProfileId);

                }
                return new ProfileAssessmentResponse(data);
            }
            catch (Exception e)
            {
                return new ProfileAssessmentResponse(e);
            }
        }

        public async Task<IProfileAssessmentResponse> AddOrUpdateProfileWorkExperienceAsync(ProfileWorkExperienceView model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: { _userIPAddress.GetUserIP().Result }");
                var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.UserId == model.ProfileId);
                if (userInfo == null)
                {
                    return new ProfileAssessmentResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                var org = await UpdateGlpOrganizationAsync(model.OrganizationId, model.OrganizationName, userInfo);

                var jobTitle = await JobTitleAsync(model.TitleId ?? 0, model.JobTitle, userInfo);
                var lineManager = await JobTitleAsync(model.LineManagerTitleId ?? 0, model.JobTitle, userInfo);
                var fieldOfWork = await WorkFieldAsync(model.FieldOfworkId ?? 0, model.FieldOfWorkString, userInfo);

                var isAdd = false;
                var workExperience = await _appDbContext.ProfileWorkExperiences.Include(k => k.Profile)
                    .FirstOrDefaultAsync(k => k.Id == model.Id);

                if (workExperience == null)
                {
                    workExperience = new ProfileWorkExperience()
                    {
                        ProfileId = model.ProfileId,
                        Created = DateTime.Now
                    };
                    isAdd = true;
                }

                workExperience.DateFrom = model.DateFrom;
                workExperience.DateTo = model.DateTo;
                workExperience.OrganizationId = org.Id;
                workExperience.IndustryId = model.IndustryId;
                workExperience.CountryId = model.CountryId;
                workExperience.Created = DateTime.Now;
                workExperience.Modified = DateTime.Now;
                workExperience.CreatedBy = userInfo.Email;
                workExperience.ModifiedBy = userInfo.Email;
                workExperience.JobTitle = model.JobTitle;
                workExperience.EmirateItemId = model.EmirateItemId;
                workExperience.FieldOfworkId = fieldOfWork.Id;
                workExperience.TitleId = jobTitle.Id;
                workExperience.LineManagerTitleId = lineManager.Id;
                workExperience.IsSomeoneReportToYou = model.IsSomeoneReportToYou;
                workExperience.IsYouReportToSomeone = model.IsYouReportToSomeone;
                workExperience.NextPosition = model.NextPosition;
                workExperience.JobDescription = model.JobDescription;

                if (isAdd)
                {
                    await _appDbContext.ProfileWorkExperiences.AddAsync(workExperience);
                }
                await _appDbContext.SaveChangesAsync().ConfigureAwait(false);

                var experience = await _appDbContext.ProfileWorkExperiences
                    .Include(k => k.Country)
                    .Include(k => k.FieldOfwork)
                    .Include(k => k.Title)
                    .Include(k => k.EmirateItem)
                    .Include(k => k.Organization)
                    .Include(k => k.LineManagerTitle)
                    .Include(k => k.Industry)
                    .FirstOrDefaultAsync(k => k.Id == workExperience.Id).ConfigureAwait(false);

                var data = _mapper.Map<ProfileWorkExperienceView>(experience);

                data.ProfileCompletedPercentage = await _completePercentageService.UpdateProfileCompletedPercentageAsync(model.ProfileId);

                var profile = _appDbContext.Profiles.FirstOrDefault(x => x.Id == model.ProfileId);

                profile.TotalYearsOfExperinceWriteOnly = new Decimal?(CalculateTotalNumberOfExpe(profile, (List<int>)null));
                await _appDbContext.SaveChangesAsync();


                return new ProfileAssessmentResponse(data);
            }
            catch (Exception e)
            {
                return new ProfileAssessmentResponse(e);
            }

        }


        private Decimal CalculateTotalNumberOfExpe(Models.Profile profile, List<int> filters = null)
        {
            var array = profile.ProfileWorkExperiences.OrderBy(m => m.DateFrom).ToList();
            if (filters != null)
                array = profile.ProfileWorkExperiences.Where(ex => ex.Organization.OrganizationSectorTypeItemId.HasValue && filters.Any(f => f == ex.Organization.OrganizationSectorTypeItemId.Value)).OrderBy(m => m.DateFrom).ToList();
            Decimal num1 = new Decimal();
            DateTime? dateTo1;
            TimeSpan? nullable1;
            TimeSpan timeSpan;
            for (int index = 0; index < ((IEnumerable<ProfileWorkExperience>)array).Count<ProfileWorkExperience>(); ++index)
            {
                if (index == 0)
                {
                    dateTo1 = array[index].DateTo;
                    if (dateTo1.HasValue)
                    {
                        Decimal num2 = num1;
                        dateTo1 = array[index].DateTo;
                        DateTime dateFrom = array[index].DateFrom;
                        TimeSpan? nullable2;
                        if (!dateTo1.HasValue)
                        {
                            nullable1 = new TimeSpan?();
                            nullable2 = nullable1;
                        }
                        else
                            nullable2 = new TimeSpan?(dateTo1.GetValueOrDefault() - dateFrom);
                        nullable1 = nullable2;
                        timeSpan = nullable1.Value;
                        Decimal days = (Decimal)timeSpan.Days;
                        num1 = num2 + days;
                    }
                    else
                    {
                        Decimal num2 = num1;
                        timeSpan = DateTime.Now.Subtract(array[index].DateFrom);
                        Decimal days = (Decimal)timeSpan.Days;
                        num1 = num2 + days;
                    }
                }
                else
                {
                    dateTo1 = array[index - 1].DateTo;
                    if (!dateTo1.HasValue)
                    {
                        dateTo1 = array[index].DateTo;
                        if (!dateTo1.HasValue)
                        {
                            dateTo1 = array[index - 1].DateTo;
                            DateTime dateFrom = array[index].DateFrom;
                            if ((dateTo1.HasValue ? (dateTo1.GetValueOrDefault() <= dateFrom ? 1 : 0) : 0) != 0)
                                continue;
                        }
                    }
                    DateTime dateFrom1 = array[index].DateFrom;
                    dateTo1 = array[index - 1].DateTo;
                    if ((dateTo1.HasValue ? (dateFrom1 >= dateTo1.GetValueOrDefault() ? 1 : 0) : 0) != 0)
                    {
                        dateTo1 = array[index].DateTo;
                        if (dateTo1.HasValue)
                        {
                            Decimal num2 = num1;
                            dateTo1 = array[index].DateTo;
                            DateTime dateFrom2 = array[index].DateFrom;
                            TimeSpan? nullable2;
                            if (!dateTo1.HasValue)
                            {
                                nullable1 = new TimeSpan?();
                                nullable2 = nullable1;
                            }
                            else
                                nullable2 = new TimeSpan?(dateTo1.GetValueOrDefault() - dateFrom2);
                            nullable1 = nullable2;
                            timeSpan = nullable1.Value;
                            Decimal days = (Decimal)timeSpan.Days;
                            num1 = num2 + days;
                        }
                        else
                        {
                            dateTo1 = array[index].DateTo;
                            if (!dateTo1.HasValue)
                            {
                                Decimal num2 = num1;
                                timeSpan = DateTime.Now.Subtract(array[index].DateFrom);
                                Decimal days = (Decimal)timeSpan.Days;
                                num1 = num2 + days;
                            }
                        }
                    }
                    else
                    {
                        dateTo1 = array[index - 1].DateTo;
                        if (dateTo1.HasValue)
                        {
                            dateTo1 = array[index].DateTo;
                            if (!dateTo1.HasValue)
                            {
                                Decimal num2 = num1;
                                DateTime now = DateTime.Now;
                                ref DateTime local = ref now;
                                dateTo1 = array[index - 1].DateTo;
                                DateTime dateTime = dateTo1.Value;
                                timeSpan = local.Subtract(dateTime);
                                Decimal days = (Decimal)timeSpan.Days;
                                num1 = num2 + days;
                                continue;
                            }
                        }
                        dateTo1 = array[index - 1].DateTo;
                        if (dateTo1.HasValue)
                        {
                            dateTo1 = array[index].DateTo;
                            if (dateTo1.HasValue)
                            {
                                Decimal num2 = num1;
                                dateTo1 = array[index].DateTo;
                                DateTime? dateTo2 = array[index - 1].DateTo;
                                TimeSpan? nullable2;
                                if (!(dateTo1.HasValue & dateTo2.HasValue))
                                {
                                    nullable1 = new TimeSpan?();
                                    nullable2 = nullable1;
                                }
                                else
                                    nullable2 = new TimeSpan?(dateTo1.GetValueOrDefault() - dateTo2.GetValueOrDefault());
                                nullable1 = nullable2;
                                timeSpan = nullable1.Value;
                                Decimal days = (Decimal)timeSpan.Days;
                                num1 = num2 + days;
                            }
                        }
                    }
                }
            }
            return Math.Round(Decimal.Parse(Math.Truncate(num1 / new Decimal(365)).ToString() + "." + (object)Math.Truncate(num1 % new Decimal(365) / new Decimal(30))), 1);
        }

        public async Task<IProfileAssessmentResponse> DeleteProfileWorkExperienceAsync(int profileExperienceId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {profileExperienceId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var data = new ProfileWorkExperienceView();
                var profileWorkExperience =
                    await _appDbContext.ProfileWorkExperiences.FirstOrDefaultAsync(k => k.Id == profileExperienceId);

                if (profileWorkExperience != null)
                {
                    _appDbContext.ProfileWorkExperiences.Remove(profileWorkExperience);
                    await _appDbContext.SaveChangesAsync();

                    data.ProfileCompletedPercentage = await _completePercentageService.UpdateProfileCompletedPercentageAsync(profileWorkExperience.ProfileId);

                    var profile = _appDbContext.Profiles.FirstOrDefault(x => x.Id == profileWorkExperience.ProfileId);

                    profile.TotalYearsOfExperinceWriteOnly = new Decimal?(CalculateTotalNumberOfExpe(profile, (List<int>)null));
                    await _appDbContext.SaveChangesAsync();
                }

                return new ProfileAssessmentResponse(data);
            }
            catch (Exception e)
            {
                return new ProfileAssessmentResponse(e);
            }
        }



        public async Task<IProfileAssessmentResponse> AddOrUpdateSkillsAndInterestAsync(
            SkillAndInterestView model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userInfo = await _appDbContext.UserInfos.FirstOrDefaultAsync(k => k.UserId == model.ProfileId);
                if (userInfo == null)
                {
                    return new ProfileAssessmentResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }


                var previousInterested = await
                    _appDbContext.ProfileInterests.Where(k => k.ProfileId == model.ProfileId).ToListAsync();

                await AddNewInterestAsync(model, previousInterested);
                await RemoveExistingInterestAsync(model, previousInterested);


                var previousSkills = await
                    _appDbContext.ProfileSkillProfiles.Where(k => k.ProfileId == model.ProfileId).ToListAsync();

                var currentSkills = await AddProfileSkillsAsync(model, userInfo, previousSkills);
                await RemoveSkillAsync(currentSkills, previousSkills);


                model.LanguageKnown = _mapper.Map<List<LanguageItemView>>(await _appDbContext.ProfileLanguage.Include(k => k.LookupLanguage)
                    .Include(m => m.LookupProficiency).Where(k => k.ProfileId == model.ProfileId).ToListAsync());

                return new ProfileAssessmentResponse(model);
            }
            catch (Exception e)
            {
                return new ProfileAssessmentResponse(e);
            }
        }

        public async Task<IProfileAssessmentResponse> AddOrUpdateSkillsAsync(
            SkillAndInterestView model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userInfo = await _appDbContext.UserInfos.FirstOrDefaultAsync(k => k.UserId == model.ProfileId);
                if (userInfo == null)
                {
                    return new ProfileAssessmentResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }


                var previousSkills = await
                    _appDbContext.ProfileSkillProfiles.Where(k => k.ProfileId == model.ProfileId).ToListAsync();

                var currentSkills = await AddProfileSkillsAsync(model, userInfo, previousSkills);
                await RemoveSkillAsync(currentSkills, previousSkills);


                model.LanguageKnown = _mapper.Map<List<LanguageItemView>>(await _appDbContext.ProfileLanguage.Include(k => k.LookupLanguage)
                    .Include(m => m.LookupProficiency).Where(k => k.ProfileId == model.ProfileId).ToListAsync());

                var skills = await _appDbContext.ProfileSkillProfiles.Include(k=>k.ProfileSkill).Where(k => k.ProfileId == model.ProfileId).ToListAsync();


                var skillViews = skills.Select(k => new ProfileSkillView()
                {
                    Id = k.ProfileSkill.Id,
                    Created = k.ProfileSkill.Created,
                    CreatedBy = k.ProfileSkill.CreatedBy,
                    Name = k.ProfileSkill.Name,
                    Modified = k.ProfileSkill.Modified,
                    ModifiedBy = k.ProfileSkill.ModifiedBy
                }).ToList();

                model.ProfileSkillItems = skillViews;

                return new ProfileAssessmentResponse(model);
            }
            catch (Exception e)
            {
                return new ProfileAssessmentResponse(e);
            }
        }

        public async Task<IProfileAssessmentResponse> AddOrUpdateInterestAsync(
            SkillAndInterestView model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userInfo = await _appDbContext.UserInfos.FirstOrDefaultAsync(k => k.UserId == model.ProfileId);
                if (userInfo == null)
                {
                    return new ProfileAssessmentResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }


                var previousInterested = await
                    _appDbContext.ProfileInterests.Where(k => k.ProfileId == model.ProfileId).ToListAsync();

                await AddNewInterestAsync(model, previousInterested);
                await RemoveExistingInterestAsync(model, previousInterested);

                model.LanguageKnown = _mapper.Map<List<LanguageItemView>>(await _appDbContext.ProfileLanguage.Include(k => k.LookupLanguage)
                    .Include(m => m.LookupProficiency).Where(k => k.ProfileId == model.ProfileId).ToListAsync());

                return new ProfileAssessmentResponse(model);
            }
            catch (Exception e)
            {
                return new ProfileAssessmentResponse(e);
            }
        }

        public async Task<IProfileAssessmentResponse> AddOrUpdateLearningPreferenceAsync(int profileId,
            List<ProfileLearningPreferenceView> models)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() } userid: {profileId} input: {models.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userInfo =
                   await _appDbContext.UserInfos.FirstOrDefaultAsync(k => k.UserId == profileId);

                if (userInfo == null)
                {
                    return new ProfileAssessmentResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                var preferences =
                   await _appDbContext.ProfileLearningPreferences.Where(k => k.ProfileId == profileId).ToListAsync();


                _appDbContext.RemoveRange(preferences);
                await _appDbContext.SaveChangesAsync();


                if (models != null && models.Any())
                {

                    var dbData = models.Select(k => new ProfileLearningPreference()
                    {

                        CreatedBy = userInfo.Email,
                        ModifiedBy = userInfo.Email,
                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        ProfileId = k.ProfileId,
                        ItemOrder = k.ItemOrder,
                        LearningPreferenceItemId = k.LearningPreferenceItemId

                    });
                    await _appDbContext.ProfileLearningPreferences.AddRangeAsync(dbData);
                    await _appDbContext.SaveChangesAsync();
                }


                return new ProfileAssessmentResponse(true, "Success", HttpStatusCode.Created);

            }
            catch (Exception e)
            {
                return new ProfileAssessmentResponse(e);
            }
        }

        public async Task<IProfileAssessmentResponse> AddOrUpdateProfileAchievementAsync(ProfileAchievementView model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.UserId == model.ProfileId);
                if (userInfo == null)
                {
                    return new ProfileAssessmentResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                GlpOrganization org = null;
                if (!string.IsNullOrWhiteSpace(model.OrganizationName))
                {
                    org = await UpdateGlpOrganizationAsync(model.OrgnizationId ?? 0, model.OrganizationName, userInfo);
                }


                var profileAchievement = await _appDbContext.ProfileAchievements.Include(k => k.Profile)
                    .FirstOrDefaultAsync(k => k.Id == model.Id);
                var isAdd = false;
                if (profileAchievement == null)
                {
                    profileAchievement = new ProfileAchievement()
                    {
                        ProfileId = model.ProfileId,
                        Created = DateTime.Now,
                        CreatedBy = userInfo.Email

                    };
                    isAdd = true;
                }

                profileAchievement.AwardItemId = model.AwardItemId;
                profileAchievement.CreatedBy = userInfo.Email;
                profileAchievement.Modified = DateTime.Now;
                profileAchievement.Date = model.Date;
                profileAchievement.OrgnizationId = org?.Id;
                profileAchievement.Description = model.Description;
                profileAchievement.Role = model.Role;
                profileAchievement.ProjectTitleAndEvent = model.ProjectTitleAndEvent;
                profileAchievement.VerbItemId = model.VerbItemId;
                profileAchievement.ImpactItemId = model.ImpactItemId;
                profileAchievement.MedalItemId = model.MedalItemId;
                profileAchievement.ReachedItemId = model.ReachedItemId;
                profileAchievement.ModifiedBy = userInfo.Email;

                if (isAdd)
                {
                    await _appDbContext.ProfileAchievements.AddAsync(profileAchievement);
                }

                await _appDbContext.SaveChangesAsync();

                var achievement = await _appDbContext.ProfileAchievements
                    .Include(k => k.Orgnization)
                    .Include(k => k.AwardItem)
                    .Include(k => k.ImpactItem)
                    .Include(k => k.MedalItem)
                    .Include(k => k.ReachedItem)
                    .Include(k => k.VerbItem)
                    .FirstOrDefaultAsync(k => k.Id == profileAchievement.Id);

                var data = _mapper.Map<ProfileAchievementView>(achievement);

                data.ProfileCompletedPercentage = await _completePercentageService.UpdateProfileCompletedPercentageAsync(model.ProfileId);

                return new ProfileAssessmentResponse(data);
            }
            catch (Exception e)
            {
                return new ProfileAssessmentResponse(e);
            }


        }


        public async Task<IProfileAssessmentResponse> DeleteProfileAchievementAsync(int achievementId)
        {
            try
            {
                var data = new ProfileAchievementView();
                var profileAchievement =
                    await _appDbContext.ProfileAchievements.FirstOrDefaultAsync(k => k.Id == achievementId);

                if (profileAchievement != null)
                {
                    _appDbContext.ProfileAchievements.Remove(profileAchievement);
                    await _appDbContext.SaveChangesAsync();

                    data.ProfileCompletedPercentage = await _completePercentageService.UpdateProfileCompletedPercentageAsync(profileAchievement.ProfileId);
                }
                return new ProfileAssessmentResponse(data);
            }
            catch (Exception e)
            {
                return new ProfileAssessmentResponse(e);
            }
        }


        public async Task<IProfileAssessmentResponse> AddOrUpdateProfileMembershipAsync(ProfileMembershipView model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.UserId == model.ProfileId);
                if (userInfo == null)
                {
                    return new ProfileAssessmentResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                var org = await UpdateGlpOrganizationAsync(model.OrganizationId, model.OrganizationName, userInfo);

                var profileMembership = await _appDbContext.ProfileMemberships.Include(k => k.Profile)
                    .FirstOrDefaultAsync(k => k.Id == model.Id);
                var isAdd = false;
                if (profileMembership == null)
                {
                    profileMembership = new ProfileMembership()
                    {
                        ProfileId = model.ProfileId,
                        Created = DateTime.Now,
                        CreatedBy = userInfo.Email

                    };
                    isAdd = true;
                }

                profileMembership.CreatedBy = userInfo.Email;
                profileMembership.Modified = DateTime.Now;
                profileMembership.Date = model.Date;
                profileMembership.OrganizationId = org.Id;
                profileMembership.Role = model.Role;
                profileMembership.ModifiedBy = userInfo.Email;

                if (isAdd)
                {
                    await _appDbContext.ProfileMemberships.AddAsync(profileMembership);
                }

                await _appDbContext.SaveChangesAsync();

                var member = await _appDbContext.ProfileMemberships
                    .Include(k => k.Organization)
                    .FirstOrDefaultAsync(k => k.Id == profileMembership.Id);

                return new ProfileAssessmentResponse(_mapper.Map<ProfileMembershipView>(member));
            }
            catch (Exception e)
            {
                return new ProfileAssessmentResponse(e);
            }


        }

        public async Task<IProfileAssessmentResponse> DeleteProfileMembershipAsync(int membershipId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {membershipId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var membership =
                    await _appDbContext.ProfileMemberships.FirstOrDefaultAsync(k => k.Id == membershipId);
                if (membership != null)
                {
                    _appDbContext.ProfileMemberships.Remove(membership);
                    await _appDbContext.SaveChangesAsync();
                }

                return new ProfileAssessmentResponse();
            }
            catch (Exception e)
            {
                return new ProfileAssessmentResponse(e);
            }
        }
        public async Task<IProfileAssessmentResponse> AddOrUpdateProfileTrainingAsync(ProfileTrainingView model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.UserId == model.ProfileId);
                if (userInfo == null)
                {
                    return new ProfileAssessmentResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                var org = await UpdateGlpOrganizationAsync(model.OrganizationId, model.OrganizationName, userInfo);

                var profileTraining = await _appDbContext.ProfileTrainings.Include(k => k.Profile)
                    .FirstOrDefaultAsync(k => k.Id == model.Id);
                var isAdd = false;
                if (profileTraining == null)
                {
                    profileTraining = new ProfileTraining()
                    {
                        ProfileId = model.ProfileId,
                        Created = DateTime.Now,
                        CreatedBy = userInfo.Email

                    };
                    isAdd = true;
                }

                profileTraining.CreatedBy = userInfo.Email;
                profileTraining.Modified = DateTime.Now;
                profileTraining.Date = model.Date;
                profileTraining.OrganizationId = org.Id;
                profileTraining.ModifiedBy = userInfo.Email;
                profileTraining.HaveCertificate = model.HaveCertificate;
                profileTraining.Title = model.Title;

                if (isAdd)
                {
                    await _appDbContext.ProfileTrainings.AddAsync(profileTraining);
                }
                await _appDbContext.SaveChangesAsync();

                var training = await _appDbContext.ProfileTrainings
                    .Include(k => k.Organization)
                    .FirstOrDefaultAsync(k => k.Id == profileTraining.Id);


                var data = _mapper.Map<ProfileTrainingView>(training);

                data.ProfileCompletedPercentage = await _completePercentageService.UpdateProfileCompletedPercentageAsync(model.ProfileId);

                return new ProfileAssessmentResponse(data);
            }
            catch (Exception e)
            {
                return new ProfileAssessmentResponse(e);
            }


        }

        public async Task<IProfileAssessmentResponse> DeleteProfileTrainingAsync(int trainingId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {trainingId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var data = new ProfileTrainingView();
                var profileTraining =
                    await _appDbContext.ProfileTrainings.FirstOrDefaultAsync(k => k.Id == trainingId);
                var appTraining = _appDbContext.ApplicationTrainings.Where(a => a.TrainingId == trainingId);
                if (profileTraining != null)
                {
                    if (appTraining != null)
                        _appDbContext.ApplicationTrainings.RemoveRange(appTraining);
                    _appDbContext.ProfileTrainings.Remove(profileTraining);
                    await _appDbContext.SaveChangesAsync();
                    data.ProfileCompletedPercentage = await _completePercentageService.UpdateProfileCompletedPercentageAsync(profileTraining.ProfileId);
                }

                return new ProfileAssessmentResponse(data);
            }
            catch (Exception e) 
            {
                logger.Error(e);
                return new ProfileAssessmentResponse(e);
            }
        }

        private async Task<List<ProfileSkillView>> AddProfileSkillsAsync(SkillAndInterestView model, UserInfo userInfo, List<ProfileSkillProfile> previousSkills)
        {

            var profileSkills = new List<ProfileSkillView>();
            foreach (var skill in model.ProfileSkillItems)
            {
                var profileSkill =
                    await _appDbContext.ProfileSkills.FirstOrDefaultAsync(k => k.Name.Equals(skill.Name)) ??
                    (await _appDbContext.ProfileSkills.AddAsync(new ProfileSkill()
                    {
                        Name = skill.Name,
                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        ModifiedBy = userInfo.Email,
                        CreatedBy = userInfo.Email
                    })).Entity;
                await _appDbContext.SaveChangesAsync();

                profileSkills.Add(_mapper.Map<ProfileSkillView>(profileSkill));

                var skillProfile = previousSkills.FirstOrDefault(k =>
                    k.ProfileId == model.ProfileId && k.Id == profileSkill.Id);

                if (skillProfile != null) continue;

                await _appDbContext.ProfileSkillProfiles.AddAsync(new ProfileSkillProfile()
                { Id = profileSkill.Id, ProfileId = model.ProfileId });
                await _appDbContext.SaveChangesAsync();
            }

            return profileSkills;
        }

        private async Task RemoveSkillAsync(List<ProfileSkillView> currentList,
            List<ProfileSkillProfile> previousSkills)
        {
            foreach (var skill in previousSkills)
            {
                var isExist = currentList.Any(k => k.Id == skill.Id);
                if (isExist) continue;

                var data = await _appDbContext.ProfileSkillProfiles.FirstOrDefaultAsync(k => k.Id == skill.Id && k.ProfileId == skill.ProfileId);
                if (data != null)
                {
                    _appDbContext.ProfileSkillProfiles.Remove(data);
                    await _appDbContext.SaveChangesAsync();
                }

            }

        }

        private async Task AddNewInterestAsync(SkillAndInterestView model, List<ProfileInterest> previousInterested)
        {
            foreach (var interestedItem in model.InterestedItems)
            {
                var interest = previousInterested.FirstOrDefault(k =>
                    k.ProfileId == model.ProfileId && k.Id == interestedItem.Id);

                if (interest != null) continue;
                await _appDbContext.ProfileInterests.AddAsync(new ProfileInterest()
                { Id = interestedItem.Id, ProfileId = model.ProfileId });
                await _appDbContext.SaveChangesAsync();
            }
        }


        private async Task RemoveExistingInterestAsync(SkillAndInterestView model, List<ProfileInterest> previousInterested)
        {
            foreach (var interestedItem in previousInterested)
            {
                var interest = model.InterestedItems.FirstOrDefault(k =>
                    k.Id == interestedItem.Id);
                if (interest != null) continue;
                _appDbContext.ProfileInterests.Remove(interestedItem);
                await _appDbContext.SaveChangesAsync();
            }
        }

        private async Task<GlpOrganization> UpdateGlpOrganizationAsync(int orgId, string organizationName, UserInfo user)
        {
            var org = await _appDbContext.GlpOrganizations.FirstOrDefaultAsync(k => k.Id == orgId) ?? (organizationName.IsArabicString()
                          ? await _appDbContext.GlpOrganizations.FirstOrDefaultAsync(k => k.NameAr == organizationName.Trim())
                          : await _appDbContext.GlpOrganizations.FirstOrDefaultAsync(k => k.NameEn == organizationName.Trim()));


            if (org == null)
            {
                org = new GlpOrganization()
                {
                    NameAr = organizationName,
                    NameEn = organizationName,
                    CreatedBy = user.Email,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    ModifiedBy = user.Email
                };
                org = (await _appDbContext.GlpOrganizations.AddAsync(org)).Entity;
                await _appDbContext.SaveChangesAsync();
            }

            return org;
        }

        private async Task<ProfileEducationFieldOfStudy> FieldOfStudyAsync(ProfileEducationView model, UserInfo user)
        {
            var fieldOfStudy = await _appDbContext.ProfileEducationFieldOfStudys.FirstOrDefaultAsync(k => k.Id == model.FieldOfStudyId) ?? (model.FieldOfStudyString.IsArabicString()
                          ? await _appDbContext.ProfileEducationFieldOfStudys.FirstOrDefaultAsync(k => k.TitleAr == model.FieldOfStudyString.Trim())
                          : await _appDbContext.ProfileEducationFieldOfStudys.FirstOrDefaultAsync(k => k.TitleEn == model.FieldOfStudyString.Trim())); ;

            if (fieldOfStudy == null)
            {
                fieldOfStudy = new ProfileEducationFieldOfStudy()
                {
                    TitleEn = model.FieldOfStudyString,
                    TitleAr = model.FieldOfStudyString,
                    CreatedBy = user.Email,
                    Modified = DateTime.Now,
                    Created = DateTime.Now,
                    ModifiedBy = user.Email
                };
                fieldOfStudy = (await _appDbContext.ProfileEducationFieldOfStudys.AddAsync(fieldOfStudy)).Entity;
                await _appDbContext.SaveChangesAsync();
            }

            return fieldOfStudy;
        }

        private async Task<ProfileWorkExperienceJobTitle> JobTitleAsync(int id, string title, UserInfo user)
        {
            var jobTitle = await _appDbContext.ProfileWorkExperienceJobTitle.FirstOrDefaultAsync(k => k.Id == id);

            if (jobTitle == null && !string.IsNullOrWhiteSpace(title))
            {
                jobTitle = new ProfileWorkExperienceJobTitle()
                {
                    TitleEn = title,
                    TitleAr = title,
                    CreatedBy = user.Email,
                    Modified = DateTime.Now,
                    Created = DateTime.Now,
                    ModifiedBy = user.Email
                };
                jobTitle = (await _appDbContext.ProfileWorkExperienceJobTitle.AddAsync(jobTitle)).Entity;
                await _appDbContext.SaveChangesAsync();
            }

            return jobTitle;
        }

        private async Task<WorkField> WorkFieldAsync(int id, string title, UserInfo user)
        {
            var workField = await _appDbContext.WorkFields.FirstOrDefaultAsync(k => k.Id == id) ?? (title.IsArabicString()
                          ? await _appDbContext.WorkFields.FirstOrDefaultAsync(k => k.NameAr == title.Trim())
                          : await _appDbContext.WorkFields.FirstOrDefaultAsync(k => k.NameEn == title.Trim()));

            if (workField == null && !string.IsNullOrWhiteSpace(title))
            {
                workField = new WorkField()
                {
                    NameEn = title,
                    NameAr = title,
                    CreatedBy = user.Email,
                    Modified = DateTime.Now,
                    Created = DateTime.Now,
                    ModifiedBy = user.Email
                };
                workField = (await _appDbContext.WorkFields.AddAsync(workField)).Entity;
                await _appDbContext.SaveChangesAsync();
            }

            return workField;
        }

        private decimal GetFileSize(long length)
        {
            if (length == 0L)
                return decimal.Zero;
            decimal num = Convert.ToDecimal(Math.Pow(1024.0, 2.0));
            return Math.Round(Convert.ToDecimal(length) / num, 3);
        }

        private static byte[] StreamToBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
