using System.Threading.Tasks;
using Uaeglp.Contracts;
using Uaeglp.Utilities;
using Microsoft.EntityFrameworkCore;
using Uaeglp.Contract.Communication;
using Uaeglp.Services.Communication;
using Microsoft.Extensions.Options;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Uaeglp.Models;
using Uaeglp.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using Microsoft.IdentityModel.Tokens;
using NLog;
using Uaeglp.Contracts.Communication;
using Uaeglp.Repositories;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;
using IdentityModel.Client;
using System.Net;
using System.ServiceModel;
using Uaeglp.Services.WebService;
using System.ServiceModel.Channels;
using Microsoft.AspNetCore.Http;

namespace Uaeglp.Services
{
    public class AccountService : BaseService, IAccountService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly AppDbContext _appDbContext;
        private readonly PasswordHashing _hashing;
        private readonly AppSettings _settings;
        private readonly EIDValidation _eidSettings;
        private readonly IMapper _mapper;
        private readonly RandomStringGeneratorService _randomStringGenerator;
        private readonly IProfilePercentageCalculationService _percentageCalculationService;
        private readonly IEmailService _emailService;
        private readonly IUserIPAddress _userIPAddress;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountService(AppDbContext appDbContext, IOptions<AppSettings> settings, IHttpContextAccessor httpContextAccessor, IOptions<EIDValidation> eidSettings, IMapper mapper, RandomStringGeneratorService randomStringGenerator, PasswordHashing hashing, IProfilePercentageCalculationService percentageCalculationService, IEmailService emailService, IUserIPAddress userIPAddress)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _randomStringGenerator = randomStringGenerator;
            _hashing = hashing;
            _percentageCalculationService = percentageCalculationService;
            _emailService = emailService;
            _settings = settings.Value;
            _eidSettings = eidSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _userIPAddress = userIPAddress;
        }

        #region Service Method

        public async Task<IAccountResponse> SignupAsync(UserRegistration view)
        {

            try
            {

                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input : {view.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                if (!view.IsTC_Accepted)
                {
                    view.Message = "Please Accept Terms And Condition";
                    view.CustomStatusCode = CustomStatusCode.FunctionalError;
                    view.Success = false;
                    return new AccountResponse(view);
                }
                //var hostname = _httpContextAccessor.HttpContext.Request.Host.Value;
               // logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  EIDForDEV : {_eidSettings.EIDForDEV}");

                

                if (_eidSettings.EIDForDEV != "true")
                {
                    //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  EIDForDEV : {_eidSettings.EIDForDEV} EmiratesID : {view.EmiratesId}");
                    
                    
                    var data = await CheckEID(view.EmiratesId);


                    if (data == CitizinType.Expat)
                    {
                        view.Message = ClientMessageConstant.EIDNotEligible;
                        view.CustomStatusCode = CustomStatusCode.Expat;
                        view.Success = false;
                        logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  EIDResponse : {view.Message}");
                        return new AccountResponse(view);
                    }
                    else if (data == CitizinType.NotFound)
                    {
                        view.Message = ClientMessageConstant.EIDNotFound;
                        view.CustomStatusCode = CustomStatusCode.NotFound;
                        view.Success = false;
                        logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  EIDResponse : {view.Message}");
                        return new AccountResponse(view);
                    }
                    else if (data == CitizinType.APINotWorking)
                    {
                        view.Message = ClientMessageConstant.EIDAPINotWorking;
                        view.CustomStatusCode = CustomStatusCode.APINotWorking;
                        view.Success = false;
                        logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  EIDResponse : {view.Message}");
                        return new AccountResponse(view);
                    }

                }

                if (_eidSettings.EIDForDEV == "true" ) 
                {
                    if(long.Parse(view.EmiratesId) < 600000000000000L)
                    {
                        view.Message = ClientMessageConstant.EIDNotEligible;
                        view.CustomStatusCode = CustomStatusCode.Expat;
                        view.Success = false;
                        logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  EIDResponse : {view.Message}");
                        return new AccountResponse(view);
                    }

                }
                var userProfiles = await _appDbContext.Profiles.FirstOrDefaultAsync(x => x.Eid == view.EmiratesId);

                    if (_appDbContext.UserInfos.Include(k => k.User).Any(o => o.User.Username == view.Email && o.TokenIsCompleted))
                    {
                        view.Message = ClientMessageConstant.EmailIdExists;
                        view.CustomStatusCode = CustomStatusCode.EmailIDExists;
                        view.Success = false;
                        return new AccountResponse(view);
                    }

                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  UserProfiles : {userProfiles}");
                if (userProfiles != null)
                {
                    view.Message = ClientMessageConstant.EIDExist;
                    view.CustomStatusCode = CustomStatusCode.EIDExist;
                    view.Success = false;
                    logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  EIDResponse : {view.CustomStatusCode}");
                    return new AccountResponse(view);


                    //var userInfo = await _appDbContext.UserInfos.Include(k => k.User)
                    //    .FirstOrDefaultAsync(x => x.UserId == userProfiles.Id);

                    //if (userInfo.TokenIsCompleted)
                    //{
                    //    view.Success = false;
                    //    view.Message = ClientMessageConstant.EmiratesIdExist;
                    //    view.CustomStatusCode = CustomStatusCode.EmiratesIDExist;
                    //    return new AccountResponse(view);
                    //}

                    //var userName = view.FirstName + " " + view.LastName;

                    //var emailResult = await _emailService.SendActivationEmailAsync(userInfo.Email, userName, view.LanguageType);

                    //userInfo.OTP = emailResult.OTP;
                    //await _appDbContext.SaveChangesAsync();


                    //view.Id = userInfo.Id;
                    //view.Message = ClientMessageConstant.Success;
                    //view.CustomStatusCode = CustomStatusCode.Success;
                    //view.Success = true;
                    //return new AccountResponse(view);
                }

                using (var transaction = _appDbContext.Database.BeginTransaction())
                {
                    try
                    {

                        var emailResult = await _emailService.SendActivationEmailAsync(view.Email, $"{view.FirstName} {view.LastName}", view.LanguageType);

                        var user = await AddUser(view);
                        var userInfo = await AddUserInfo(user, view.Email, emailResult.OTP);
                        var profile = await AddProfile(view, user, view.EmiratesId);
                        await PermissionSetAsync(user.Id, 2);

                        view.Id = user.Id;
                        view.Message = ClientMessageConstant.Success;
                        view.CustomStatusCode = CustomStatusCode.Success;
                        view.Success = true;


                        transaction.Commit();
                        return new AccountResponse(view);

                    }
                    catch (System.Exception ex)
                    {
                        // TODO logging stuff
                        transaction.Rollback();
                        view.Success = false;
                        view.Message = ClientMessageConstant.WeAreUnableToProcessYourRequest;
                        view.CustomStatusCode = CustomStatusCode.FunctionalError;
                        new AccountResponse(ex);
                        return new AccountResponse(view);
                    }

                }


            }
            catch (Exception ex1)
            {
                view.Success = false;
                view.Message = ClientMessageConstant.WeAreUnableToProcessYourRequest;
                view.CustomStatusCode = CustomStatusCode.FunctionalError;

                return new AccountResponse(view, ex1);


            }
        }

        public async Task<CitizinType> CheckEID(string eid)
        {
            try
            {
                var appSetting = new EIDValidation()
                {
                    user_name = _eidSettings.user_name,
                    password = _eidSettings.password,
                    gsb_apikey = _eidSettings.gsb_apikey,
                    EndpointURL = _eidSettings.EndpointURL
                };

                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  UserName: {_eidSettings.user_name} Password: {_eidSettings.password} GSB_APIKey: {_eidSettings.gsb_apikey} EndpointURL: {_eidSettings.EndpointURL}");
                
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.Name = "CitizenVerification_bind";
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Basic;

                EndpointAddress endpoint = new EndpointAddress(appSetting.EndpointURL);
                


                CitizenVerification_pttClient verificationPttClient = new CitizenVerification_pttClient(binding, endpoint);
                verificationPttClient.ClientCredentials.UserName.UserName = appSetting.user_name;
                verificationPttClient.ClientCredentials.UserName.Password = appSetting.password;
                using (new OperationContextScope((IContextChannel)verificationPttClient.InnerChannel))
                {
                    HttpRequestMessageProperty requestMessageProperty = new HttpRequestMessageProperty();
                    requestMessageProperty.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(verificationPttClient.ClientCredentials.UserName.UserName + ":" + verificationPttClient.ClientCredentials.UserName.Password));
                    requestMessageProperty.Headers["GSB-APIKey"] = appSetting.gsb_apikey;
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = (object)requestMessageProperty;
                    CitizenVerificationResponse verificationResponse1 = new CitizenVerificationResponse();
                    try
                    {
                        CitizenVerificationResponse verificationResponse2 = verificationPttClient.CitizenVerification(new CitizenVerificationRequest()
                        {
                            EmiratesIDNumber = eid
                        });
                        if (verificationResponse2.VerificationResult == ResultType.Eligible)
                            return CitizinType.Citizen;
                        if (verificationResponse2.VerificationResult == ResultType.NotEligible)
                            return CitizinType.Expat;
                        if (verificationResponse2.VerificationResult == ResultType.NoDataFound)
                            return CitizinType.NotFound;
                        throw new Exception("Unknown Verification Result received from the API.");
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        return CitizinType.APINotWorking;
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }


        public async Task<IAccountResponse> ResetPassword(ResetPassword view)
        {

            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input : {view.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

                var userInfo = await _appDbContext.UserInfos.FirstOrDefaultAsync(x => x.UserId == view.UserId);
                if (userInfo == null)
                {

                    return new AccountResponse(new ViewModels.UserRegistration
                    {
                        Id = view.UserId,
                        Success = false,
                        CustomStatusCode = CustomStatusCode.UserNotExist,
                        Message = ClientMessageConstant.UserNotFound

                    });
                }

                var passresult = _hashing.ValidatePassword(view.OldPassword, userInfo.Password);
                if (!passresult)
                {

                    return new AccountResponse(new ViewModels.UserRegistration
                    {
                        Id = userInfo.UserId,
                        Success = false,
                        CustomStatusCode = CustomStatusCode.PasswordIsWrong,
                        Message = ClientMessageConstant.PasswordIsWrong

                    });
                }

                userInfo.Password = _hashing.CreateHash(view.NewPassword);
                await _appDbContext.SaveChangesAsync();


                return new AccountResponse(new ViewModels.UserRegistration
                {
                    Id = userInfo.UserId,
                    Success = true,
                    CustomStatusCode = CustomStatusCode.Success,
                    Message = ClientMessageConstant.PasswordUpdatedSuccessfully

                });
            }
            catch (System.Exception ex)
            {
                return new AccountResponse(new ViewModels.UserRegistration
                {
                    Success = false,
                    CustomStatusCode = CustomStatusCode.FunctionalError,
                    Message = ClientMessageConstant.WeAreUnableToProcessYourRequest

                }, ex);
            }
        }

        public async Task<IAccountResponse> SetNewPassword(ViewModels.SetNewPassword view)
        {

            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input : {view.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userInfo = await _appDbContext.UserInfos.FirstOrDefaultAsync(x => x.UserId == view.UserId);
                if (userInfo == null)
                {
                    //Message = ClientMessageConstant.UserNotFound);
                    return new AccountResponse(new ViewModels.UserRegistration
                    {
                        Id = view.UserId,
                        Success = false,
                        CustomStatusCode = CustomStatusCode.UserNotExist,
                        Message = ClientMessageConstant.UserNotFound

                    });
                };

                userInfo.Password = _hashing.CreateHash(view.NewPassword);
                userInfo.TokenIsCompleted = true;
                await _appDbContext.SaveChangesAsync();

                //Message = _settings.Success;
                return new AccountResponse(new ViewModels.UserRegistration
                {
                    Id = userInfo.UserId,

                    Success = true,
                    CustomStatusCode = CustomStatusCode.Success,
                    Message = ClientMessageConstant.PasswordUpdatedSuccessfully

                });
            }
            catch (System.Exception ex)
            {
                // TODO logging stuff
                //Message = $"An error occurred when resetting password: {ex.Message}";
                return new AccountResponse(new ViewModels.UserRegistration
                {
                    Success = false,
                    CustomStatusCode = CustomStatusCode.FunctionalError,
                    Message = ClientMessageConstant.WeAreUnableToProcessYourRequest
                }, ex);
            }
        }

        public async Task<IResponse<ViewModels.ValidateOtp>> ValidateOtp(ViewModels.ValidateOtp view)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input : {view.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userInfo = await _appDbContext.UserInfos.FirstOrDefaultAsync
                    (
                        x => x.UserId == view.UserId &&
                        x.OTP == view.Otp);

                if (userInfo == null)
                {
                    return new OTPResponce(new ViewModels.ValidateOtp
                    {
                        Success = false,
                        CustomStatusCode = CustomStatusCode.OTPExpiredOrInvalid,
                        Message = ClientMessageConstant.OTPExpiredOrInvalid

                    });
                }

                //userInfo.TokenIsCompleted = true;
                userInfo.IsActive = true;
                userInfo.CanLogin = true;
                userInfo.Modified = DateTime.Now;
                _appDbContext.UserInfos.Update(userInfo);
                await _appDbContext.SaveChangesAsync();

                return new OTPResponce(new ViewModels.ValidateOtp
                {
                    UserId = view.UserId,
                    Otp = view.Otp,
                    Success = true,
                    CustomStatusCode = CustomStatusCode.Success,
                    Message = ClientMessageConstant.Success

                });

            }
            catch (System.Exception ex)
            {
                //Message = $"An error occurred when resetting password: {ex.Message}";
                return new OTPResponce(new ViewModels.ValidateOtp
                {
                    UserId = view.UserId,
                    Otp = view.Otp,
                    Success = false,
                    CustomStatusCode = CustomStatusCode.FunctionalError,
                    Message = ClientMessageConstant.WeAreUnableToProcessYourRequest

                });
            }
        }

        public async Task<IResponse<ViewModels.ValidateOtp>> ResendOtp(ViewModels.ResendOTP view)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input : {view.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(x => x.UserId == view.UserId);

                if (userInfo == null)
                {
                    return new OTPResponce(new ViewModels.ValidateOtp
                    {
                        UserId = view.UserId,
                        Success = false,
                        CustomStatusCode = CustomStatusCode.UserNotExist,
                        Message = ClientMessageConstant.UserNotFound

                    });
                }
                var userName = view.LanguageType == LanguageType.EN ? userInfo.User.NameEn : userInfo.User.NameAr;

                var emailResult = view.ResendOtpType == ResendOtpType.ResetPassword
                    ? await _emailService.SendResetPasswordEmailAsync(userInfo.Email, userName, view.LanguageType)
                    : await _emailService.SendActivationEmailAsync(userInfo.Email, userName, view.LanguageType);
                userInfo.OTP = emailResult.OTP;
                userInfo.Modified = DateTime.Now;
                _appDbContext.UserInfos.Update(userInfo);
                await _appDbContext.SaveChangesAsync();


                return new OTPResponce(new ViewModels.ValidateOtp
                {
                    UserId = view.UserId,
                    Success = true,
                    CustomStatusCode = CustomStatusCode.Success,
                    Message = ClientMessageConstant.Success

                });

            }
            catch (System.Exception ex)
            {
                //Message = $"An error occurred when resetting password: {ex.Message}";
                return new OTPResponce(new ViewModels.ValidateOtp
                {
                    UserId = view.UserId,
                    Success = false,
                    CustomStatusCode = CustomStatusCode.FunctionalError,
                    Message = ClientMessageConstant.WeAreUnableToProcessYourRequest

                });
            }
        }

        public async Task<IAccountResponse> ForgotPass(ForgotPassword view)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input : {view.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(x => x.Email == view.Email);

                ViewModels.ValidateOtp validateOtp = new ViewModels.ValidateOtp();
                if (userInfo == null)
                {
                    return new AccountResponse(new ViewModels.UserRegistration
                    {
                        Success = false,
                        CustomStatusCode = CustomStatusCode.NotRegisteredEmailID,
                        Message = ClientMessageConstant.NotRegisteredEmailId

                    });
                }
                else
                {

                    var profileInfo = await _appDbContext.Profiles.FirstOrDefaultAsync(x => x.Id == userInfo.Id);

                    var userName = view.LanguageType == LanguageType.EN ? userInfo.User.NameEn : userInfo.User.NameAr;

                    var emailResult = await _emailService.SendResetPasswordEmailAsync(userInfo.Email, userName, view.LanguageType);

                    validateOtp.UserId = userInfo.UserId;
                    validateOtp.Otp = userInfo.OTP;
                    userInfo.OTP = emailResult.OTP;
                    userInfo.Modified = DateTime.Now;
                    _appDbContext.UserInfos.Update(userInfo);
                    await _appDbContext.SaveChangesAsync();




                    return new AccountResponse(new ViewModels.UserRegistration
                    {
                        Id = userInfo.UserId,
                        FirstName = profileInfo.FirstNameEn,
                        LastName = profileInfo.LastNameEn,
                        Success = true,
                        CustomStatusCode = CustomStatusCode.Success,
                        Message = ClientMessageConstant.Success
                    });
                }

            }
            catch (System.Exception ex)
            {
                //Message = $"An error occurred when resetting password: {ex.Message}";
                return new AccountResponse(new ViewModels.UserRegistration
                {
                    Success = false,
                    CustomStatusCode = CustomStatusCode.FunctionalError,
                    Message = ClientMessageConstant.WeAreUnableToProcessYourRequest

                }, ex);
            }
        }

        public async Task<IAccountResponse> ForgotEmailID(ViewModels.ForgotEmail view)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input : {view.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var profileInfo = await _appDbContext.Profiles.FirstOrDefaultAsync(x => x.Eid == view.EmirateID);

                ViewModels.ValidateOtp Votp = new ViewModels.ValidateOtp();
                if (profileInfo == null)
                {
                    return new AccountResponse(new ViewModels.UserRegistration
                    {
                        Success = false,
                        CustomStatusCode = CustomStatusCode.NotRegisteredEmiratesID,
                        Message = ClientMessageConstant.NotRegisteredEmiratesId

                    });
                }
                else
                {
                    var userInfo = await _appDbContext.UserInfos.FirstOrDefaultAsync(x => x.UserId == profileInfo.Id);
                    //EMailProvider emailSet = new EMailProvider(_settings.SmtpUsername, _settings.SmtpPassword, _settings.SmtpHost, _settings.SmtpPort);
                    //SendOtpEmail(emailSet, _settings.SmtpUsername, userInfo.Email, userInfo.otp);


                    //Votp.UserId = userInfo.UserId;
                    //Votp.Otp = userInfo.otp;

                    //Message = _settings.Success;
                    return new AccountResponse(new ViewModels.UserRegistration
                    {
                        Id = userInfo.UserId,
                        FirstName = profileInfo.FirstNameEn,
                        LastName = profileInfo.LastNameEn,
                        Email = userInfo.Email,
                        //UserId = userInfo.UserId,
                        //Otp = userInfo.otp,
                        Success = true,
                        CustomStatusCode = CustomStatusCode.Success,
                        Message = ClientMessageConstant.Success
                    });
                }

            }
            catch (System.Exception ex)
            {
                //Message = $"An error occurred when resetting password: {ex.Message}";
                return new AccountResponse(new ViewModels.UserRegistration
                {
                    Success = false,
                    CustomStatusCode = CustomStatusCode.FunctionalError,
                    Message = ClientMessageConstant.WeAreUnableToProcessYourRequest

                }, ex);
            }
        }

        public async Task<IAccountResponse> AddOrUpdateDeviceInfoAsync(UserDeviceInfoViewModel model)
        {
            try
            {
                var userDevice = await _appDbContext.UserDeviceInfos.FirstOrDefaultAsync(k => k.DeviceId == model.DeviceId);
                if (userDevice != null)
                {
                    _appDbContext.UserDeviceInfos.Remove(userDevice);
                    await _appDbContext.SaveChangesAsync();
                }

                var data = await _appDbContext.UserDeviceInfos.FirstOrDefaultAsync(k => k.DeviceId == model.DeviceId);

                if (data == null)
                {
                    data = new UserDeviceInfo()
                    {
                        UserId = model.UserId,
                        DeviceId = model.DeviceId,
                        DeviceType =(byte) model.DeviceType,
                        IsActive = model.IsActive,
                        CreatedOn = DateTime.Now
                    };

                    await _appDbContext.UserDeviceInfos.AddAsync(data);
                    await _appDbContext.SaveChangesAsync();
                    return new AccountResponse(model);
                }
                else
                {

                    data.DeviceId = model.DeviceId;
                    data.DeviceType = (byte)model.DeviceType;
                    data.IsActive = model.IsActive;
                    data.ModifiedOn = DateTime.Now;
                    await _appDbContext.SaveChangesAsync();
                }


                model.Success = true;
                model.Message = ClientMessageConstant.Success;

                return new AccountResponse(model);

            }
            catch (Exception ex)
            {

                model.Success = false;
                model.Message = ClientMessageConstant.WeAreUnableToProcessYourRequest;
                return new AccountResponse(model, ex);
            }
        }

        public async Task<IResponse<UserDetailsView>> GetUserDetailsAsync(int userId)
        {

            try
            {
                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == userId);
                if (profile == null)
                {
                    return new UserResponse(new UserDetailsView
                    {
                        UserId = userId,
                        Success = false,
                        CustomStatusCode = CustomStatusCode.UserNotExist,
                        Message = ClientMessageConstant.UserNotFound

                    });
                }

                var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == userId);
                var workExperiences = await _appDbContext.ProfileWorkExperiences.Include(k=>k.Title).Where(k => k.ProfileId == userId).ToListAsync();
                var data = new UserDetailsView()
                {
                    FirstNameAR = profile.FirstNameAr,
                    FirstNameEN = profile.FirstNameEn,
                    LastNameAR = profile.LastNameAr,
                    LastNameEN = profile.LastNameEn,
                    SecondNameAR = profile.SecondNameAr,
                    SecondNameEN = profile.SecondNameEn,
                    ThirdNameAR = profile.ThirdNameAr,
                    ThirdNameEN = profile.ThirdNameEn,
                    UserImageFileId = user.OriginalImageFileId ?? 0,
                    UserId = profile.Id,
                    HasAdminRights = await _appDbContext.PermissionSetUsers.AnyAsync(o => o.UserId == userId && o.PermissionSetId == 1),
                    HasUserRights = await _appDbContext.PermissionSetUsers.AnyAsync(o => o.UserId == userId && o.PermissionSetId == 2),
                    ProfileCompletedPercentage = await _percentageCalculationService.UpdateProfileCompletedPercentageAsync(profile.Id),
                    Success = true,
                    CustomStatusCode = CustomStatusCode.Success,
                    Message = ClientMessageConstant.Success,
                    UserGroups = await _appDbContext.NetworkGroupProfiles.Join(_appDbContext.NetworkGroups,
                                        ngp => ngp.NetworkGroupId, ng => ng.Id,
                                        (ngp, ng) => new { ngp, ng })
                                            .Where(z => z.ngp.ProfileId == userId)
                                        .Select(z => new FilterTypeViewModel
                                        {
                                            Id = z.ngp.NetworkGroupId,
                                            FilterType = z.ng.NameEn,
                                            NameEn = z.ng.NameEn,
                                            NameAr = z.ng.NameAr,
                                            DescriptionAr = z.ng.DescriptionAr,
                                            DescriptionEn = z.ng.DescriptionEn,
                                            LogoId = z.ng.LogoId ?? 0
                                        }).ToListAsync(),
                    Designation = workExperiences.OrderByDescending(k=>k.DateFrom).FirstOrDefault()?.Title?.TitleEn,
                    DesignationAr = workExperiences.OrderByDescending(k=>k.DateFrom).FirstOrDefault()?.Title?.TitleAr


                };

                data.ApplicationSetting =
                    _mapper.Map<List<ApplicationSettingViewModel>>(
                        await _appDbContext.ApplicationSettings.ToListAsync());

                return new UserResponse(data);
            }
            catch (Exception ex1)
            {
                new AccountResponse(ex1);

                return new UserResponse(new UserDetailsView
                {
                    Success = false,
                    CustomStatusCode = CustomStatusCode.FunctionalError,
                    Message = ClientMessageConstant.WeAreUnableToProcessYourRequest

                });
            }
        }



        public async Task<LoginResponseViewModel> LoginAsync(AuthenticateViewModel model)
        {

            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  Username : {model.Username} DeviceID: {model.DeviceId} DeviceType: {model.DeviceType.ToString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
            var responseModel = new LoginResponseViewModel()
            {
                Success = false,
                IsAuthorized = false,
                Message = ClientMessageConstant.InvalidUserNameAndPassword,
            };

            try
            {
                var client = new HttpClient();

                var disco = await client.GetDiscoveryDocumentAsync(_settings.AuthorizeUrl);
                if (disco.IsError)
                {
                    logger.Error($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  error : {disco.Error}");
                    return responseModel;
                }


                var pwdToken = await client.RequestPasswordTokenAsync(new PasswordTokenRequest()
                {
                    Address = _settings.AuthorizeUrl + "/connect/token",
                    UserName = model.Username,
                    Password = model.Password,
                    ClientId = _settings.ClientId,
                    ClientSecret = _settings.ClientSecret,
                    Scope = _settings.Scope
                });

                if (pwdToken.IsError)
                {
                    logger.Error($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  error : {pwdToken.Error}");
                    return responseModel;
                }

                // request token
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = _settings.ClientId,
                    ClientSecret = _settings.ClientSecret,
                    Scope = _settings.ApiScope
                });

                if (tokenResponse.IsError)
                {
                    logger.Error($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  error : {tokenResponse.Error}");
                    return responseModel;
                }

                var userInfo = await _appDbContext.UserInfos.FirstOrDefaultAsync(k => k.Email == model.Username);

                if (model.DeviceId != null)
                {
                    var deviceInfo = new UserDeviceInfoViewModel()
                    {
                        DeviceType = model.DeviceType,
                        DeviceId = model.DeviceId,
                        UserId = userInfo.UserId,
                        IsActive = true
                    };

                    await AddOrUpdateDeviceInfoAsync(deviceInfo);
                }
                


                responseModel.UserDetails = (await GetUserDetailsAsync(userInfo?.UserId ?? 0))?.Resource;

                responseModel.ExpireMinutes = tokenResponse.ExpiresIn;
                responseModel.Token = tokenResponse.AccessToken;
                responseModel.Success = true;
                responseModel.IsAuthorized = true;
                responseModel.Message = ClientMessageConstant.Success;

                return responseModel;
            }
            catch (Exception e)
            {
                new AccountResponse(e);
                responseModel.Message = ClientMessageConstant.WeAreUnableToProcessYourRequest;

                return responseModel;
            }


        }

        public async Task<bool> LogOutAsync(int userId, string deviceId)
        {
            try
            {
                var userDevice = await _appDbContext.UserDeviceInfos.FirstOrDefaultAsync(k => k.DeviceId == deviceId && k.UserId == userId);
                if (userDevice != null)
                {
                    _appDbContext.UserDeviceInfos.Remove(userDevice);
                    await _appDbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }


        //public async Task<LoginResponseViewModel> LoginAsync(AuthenticateViewModel model)
        //{
        //    var responseModel = new LoginResponseViewModel();
        //    var userInfo = await _appDbContext.UserInfos.FirstOrDefaultAsync(k =>
        //        k.Email == model.Username);
        //    if (userInfo == null)
        //    {
        //        return responseModel;
        //    }

        //    responseModel.IsAuthorized = _hashing.ValidatePassword(model.Password, userInfo.Password);

        //    if (!responseModel.IsAuthorized)
        //    {
        //        return responseModel;
        //    }

        //    // authentication successful so generate jwt token
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_settings.Secret);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //            new Claim(ClaimTypes.Name, userInfo.UserId.ToString() + " : " + userInfo.Email)
        //        }),
        //        Expires = DateTime.Now.AddMinutes(30),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);

        //    responseModel.Token = tokenHandler.WriteToken(token);

        //    return responseModel;

        //}


        #endregion

        #region Internal Methos
        private async Task<Models.UserInfo> AddUserInfo(Models.User user, string email, int otp)
        {
            var data = _appDbContext.UserInfos.Include(k => k.User).Any(o => o.User.Username == email && o.TokenIsCompleted != true);

            var userDetails = await _appDbContext.UserInfos.Where(x => x.Id == user.Id).FirstOrDefaultAsync();
            if (data)
            {
                userDetails.Id = user.Id;
                userDetails.UserId = user.Id;
                userDetails.Email = email;
                userDetails.Password = "notset";
                userDetails.Token = userDetails.Token;
                userDetails.TokenIsCompleted = false;
                userDetails.TokenCanExpire = true;
                userDetails.IsLicensed = true;
                userDetails.CreatedBy = "Anonymous User";
                userDetails.ModifiedBy = "Anonymous User";
                userDetails.Created = System.DateTime.UtcNow;
                userDetails.Modified = System.DateTime.UtcNow;
                userDetails.OTP = otp;

                await _appDbContext.SaveChangesAsync();
                return userDetails;
            }


            var userInfo = new Models.UserInfo
            {
                Id = user.Id,
                UserId = user.Id,
                Email = email,
                Password = "notset", // _hashing.CreateHash(view.Password),
                //Token = _randomStringGenerator.Generate(25),
                Token = randomNumericDigits(25),
                TokenIsCompleted = false,
                TokenCanExpire = true,
                IsLicensed = true,
                CreatedBy = "Anonymous User",
                ModifiedBy = "Anonymous User",
                Created = System.DateTime.UtcNow,
                Modified = System.DateTime.UtcNow,
                OTP = otp
            };

            _appDbContext.UserInfos.Add(userInfo);
            await _appDbContext.SaveChangesAsync();
            return userInfo;
        }

        public  string randomNumericDigits(int length)
        {
            var random = new Random();
            string randomNumber = string.Empty;
            for (int i = 0; i < length; i++)
            {
                randomNumber = String.Concat(randomNumber, random.Next(10).ToString());
            }
            return randomNumber;
        }

        private async Task<Models.Profile> AddProfile(UserRegistration view, Models.User user, string EmiratesId)
        {

            var data = _appDbContext.UserInfos.Include(k => k.User).Any(o => o.User.Username == view.Email && o.TokenIsCompleted != true);

            var userDetails = await _appDbContext.Profiles.Where(x => x.Id == user.Id).FirstOrDefaultAsync();
            if (userDetails != null)
            {
                userDetails.Id = user.Id;
                userDetails.Eid = EmiratesId;
                userDetails.CompletenessPercentage = 0;
                userDetails.PostsCount = 0;
                userDetails.FollowersCount = 0;
                userDetails.FollowingCount = 0;
                userDetails.IsInfluencer = false;
                userDetails.IsPublicFigure = false;
                userDetails.CreatedBy = view.Email;
                userDetails.ModifiedBy = view.Email;
                userDetails.Created = DateTime.Now;
                userDetails.Modified = DateTime.Now;
                userDetails.ProfileLastModified = DateTime.Now;
                userDetails.Lpspoints = 0;

                await _appDbContext.SaveChangesAsync();
                return userDetails;
            }

            var profile = new Models.Profile
            {
                Id = user.Id,
                Eid = EmiratesId,
                Created = DateTime.Now,
                Modified = DateTime.Now,
                CreatedBy = view.Email,
                ModifiedBy = view.Email,
                CompletenessPercentage = 0, // _hashing.CreateHash(view.Password),
                PostsCount = 0,
                FollowersCount = 0,
                FollowingCount = 0,
                IsInfluencer = false,
                IsPublicFigure = false,
                ProfileLastModified = DateTime.Now,
                Lpspoints = 0

            };

            if (view.FirstName.IsArabicString())
            {
                profile.FirstNameAr = view.FirstName;
                profile.LastNameAr = view.LastName;

            }
            else
            {
                profile.FirstNameEn = view.FirstName;
                profile.LastNameEn = view.LastName;
            }

            _appDbContext.Profiles.Add(profile);
            await _appDbContext.SaveChangesAsync();
            return profile;
        }

        private async Task<Models.User> AddUser(ViewModels.UserRegistration view)
        {
            var data = _appDbContext.UserInfos.Include(k => k.User).Any(o => o.User.Username == view.Email && o.TokenIsCompleted != true);

            //var data2 = _appDbContext.Profiles.Include(m )
            var userDetails = await _appDbContext.Users.Where(x => x.Username == view.Email).FirstOrDefaultAsync();
            if (data)
            {
                //userDetails.Id = userDetails.Id;
                userDetails.OrganizationId = 1;
                userDetails.Username = view.Email;
                userDetails.CreatedBy = "Anonymous User";
                userDetails.ModifiedBy = "Anonymous User";
                userDetails.Created = System.DateTime.UtcNow;
                userDetails.Modified = System.DateTime.UtcNow;
                userDetails.IsOnline = false;
                userDetails.IsAnonymous = false;
                userDetails.PermissionSid = "";
                userDetails.PermissionSetSid = "2";
                userDetails.LastUsedPermissionSetId = 2;
                userDetails.IsTC_Accepted = true;

                await _appDbContext.SaveChangesAsync();
                return userDetails;
            }

            var user = new Models.User
            {
                OrganizationId = 1,
                Username = view.Email,
                CreatedBy = "Anonymous User",
                ModifiedBy = "Anonymous User",
                Created = System.DateTime.UtcNow,
                Modified = System.DateTime.UtcNow,
                IsOnline = false,
                IsAnonymous = false,
                PermissionSid = "",
                PermissionSetSid = "2",
                LastUsedPermissionSetId = 2,
                IsTC_Accepted = true
            };

            if (view.FirstName.IsArabicString())
            {
                user.NameAr = $"{view.FirstName} {view.LastName}";
                user.LanguageKey = "ar";
            }
            else
            {
                user.NameEn = $"{view.FirstName} {view.LastName}";
                user.LanguageKey = "en";
            }


            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();
            return user;
        }

        private bool SendOtpEmail(EMailProvider emailing, string from, string to, int? otp, string UserName, string MailType)
        {
            bool sent;
            try
            {
                sent = emailing.SendOtp(from, to, otp, UserName, MailType);

            }
            catch (Exception ex)
            {
                sent = false;
            }

            return sent;
        }

        private async Task PermissionSetAsync(int userId, int permissionId)
        {

            var userData = _appDbContext.UserInfos.Include(k => k.User).Any(o => o.TokenIsCompleted != true);

            var userDetails = await _appDbContext.PermissionSetUsers.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            if (userDetails != null)
            {
                userDetails.UserId = userId;
                userDetails.PermissionSetId = permissionId;

                await _appDbContext.SaveChangesAsync();
                //return userDetails;
            }
            else
            {

                var data = new PermissionSetUser()
                {
                    UserId = userId,
                    PermissionSetId = permissionId
                };

                await _appDbContext.PermissionSetUsers.AddAsync(data);
                await _appDbContext.SaveChangesAsync();
            }
        }

        #endregion
    }
}
