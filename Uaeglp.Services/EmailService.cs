using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using Uaeglp.Contracts;
using Uaeglp.Utilities;
using Uaeglp.ViewModels;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uaeglp.Models;
using Uaeglp.Repositories;
using Uaeglp.ViewModels.Enums;
using NLog;

namespace Uaeglp.Services
{
    public class EmailService : IEmailService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly Random _random;
        private readonly AppDbContext _appDbContext;
        private readonly AppSettings _appSettings;
        private readonly IEncryptionManager _encryptor;
        private readonly IUserIPAddress _userIPAddress;

        public EmailService(AppDbContext appDbContext, IOptions<AppSettings> settings, Random random, IEncryptionManager encryption, IUserIPAddress userIPAddress)
        {
            _appDbContext = appDbContext;
            _random = random;
            _appSettings = settings.Value;
            _encryptor = encryption;
            _userIPAddress = userIPAddress;
        }

        public async Task<EmailView> SendActivationEmailAsync(string toEmail, string userName,
            LanguageType languageType = LanguageType.EN)
        {
            EmailView emailResponse = new EmailView();
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  EmailTo: {toEmail} UserName: {userName} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var emailSetting = await _appDbContext.ApplicationSettings.ToListAsync();

                var appSetting = new AppSettings()
                {
                    SmtpHost = emailSetting.FirstOrDefault(k=>k.Key == "smtpHostName")?.Value,
                    SmtpPassword = emailSetting.FirstOrDefault(k => k.Key == "smtpPassword")?.Value,
                    SmtpPort = emailSetting.FirstOrDefault(k => k.Key == "smtpPortNumber")?.Value,
                    SmtpUsername = emailSetting.FirstOrDefault(k => k.Key == "smtpUserName")?.Value
                };
                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SMTP_PasswordEncrypt: {appSetting.SmtpPassword}");

                appSetting.SmtpPassword = _encryptor.Decrypt(appSetting.SmtpPassword);

                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SMTP_PasswordDecrypt: {appSetting.SmtpPassword}");

                var email = new MailMessage(appSetting.SmtpUsername, toEmail);
                var template = await _appDbContext.Templates.Where(x => x.SysName == "ActivationTemplateMobile").FirstOrDefaultAsync();
                var templateInfo = await _appDbContext.TemplateInfos.Include(k=>k.EmailHeaderAndFooterTemplate).Where(x => x.TemplateId == template.Id).FirstOrDefaultAsync();

                var otp = GenerateOtp();

                var body = languageType == LanguageType.EN ? GetActivationEnglishContent(userName, templateInfo, otp, email) : GetActivationArabicContent(userName, templateInfo, otp, email);
              

                email.Body = body;
                email.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    smtp.Host = appSetting.SmtpHost;
                    smtp.EnableSsl = true;
                    NetworkCredential networkCred = new NetworkCredential(appSetting.SmtpUsername, appSetting.SmtpPassword);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = networkCred;
                    smtp.Port = int.Parse(appSetting.SmtpPort);
                    smtp.Send(email);
                }
                emailResponse.ID = Guid.NewGuid();
                emailResponse.Result = true;
                emailResponse.Message = "Email Sent";
                emailResponse.OTP = otp;

                return emailResponse;
            }
            catch(Exception e)
            {
                logger.Error(e);
                emailResponse.ID = Guid.NewGuid();
                emailResponse.Result = false;
                emailResponse.Message = "Email Not Sent";
                logger.Info(emailResponse);
                return emailResponse;
            }
        }

        public async Task<EmailView> SendResetPasswordEmailAsync(string toEmail, string userName,
            LanguageType languageType = LanguageType.EN)
        {
            EmailView emailResponse = new EmailView();
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  EmailTo: {toEmail} UserName: {userName} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var emailSetting = await _appDbContext.ApplicationSettings.ToListAsync();

                var appSetting = new AppSettings()
                {
                    SmtpHost = emailSetting.FirstOrDefault(k => k.Key == "smtpHostName")?.Value,
                    SmtpPassword = emailSetting.FirstOrDefault(k => k.Key == "smtpPassword")?.Value,
                    SmtpPort = emailSetting.FirstOrDefault(k => k.Key == "smtpPortNumber")?.Value,
                    SmtpUsername = emailSetting.FirstOrDefault(k => k.Key == "smtpUserName")?.Value
                };

                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SMTP_PasswordEncrypt: {appSetting.SmtpPassword}");

                appSetting.SmtpPassword = _encryptor.Decrypt(appSetting.SmtpPassword);

                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SMTP_PasswordDecrypt: {appSetting.SmtpPassword}");

                var email = new MailMessage(appSetting.SmtpUsername, toEmail);

                var template = await _appDbContext.Templates.Where(x => x.SysName == "ResetPasswordMobile").FirstOrDefaultAsync();
                var templateInfo = await _appDbContext.TemplateInfos.Include(k => k.EmailHeaderAndFooterTemplate).Where(x => x.TemplateId == template.Id).FirstOrDefaultAsync();

                var otp = GenerateOtp();

                var body = languageType == LanguageType.EN ? GetResetPasswordEnglishContent(userName, templateInfo, otp, email) : GetResetPasswordArabicContent(userName, templateInfo, otp, email);


                email.Body = body;
                email.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    smtp.Host = appSetting.SmtpHost;
                    smtp.EnableSsl = true;
                    NetworkCredential networkCred = new NetworkCredential(appSetting.SmtpUsername, appSetting.SmtpPassword);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = networkCred;
                    smtp.Port = int.Parse(appSetting.SmtpPort);
                    smtp.Send(email);
                }
                emailResponse.ID = Guid.NewGuid();
                emailResponse.Result = true;
                emailResponse.Message = "Email Sent";
                emailResponse.OTP = otp;

                return emailResponse;
            }
            catch (Exception e)
            {
                logger.Error(e);
                emailResponse.ID = Guid.NewGuid();
                emailResponse.Result = false;
                emailResponse.Message = "Email Not Sent";
                return emailResponse;
            }
        }

        public async Task<EmailView> SendProgramSubmissionEmailAsync(string toEmail, string programName, string userName)
        {
            EmailView emailResponse = new EmailView();
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  EmailTo: {toEmail} UserName: {userName} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var emailSetting = await _appDbContext.ApplicationSettings.ToListAsync();

                var appSetting = new AppSettings()
                {
                    SmtpHost = emailSetting.FirstOrDefault(k => k.Key == "smtpHostName")?.Value,
                    SmtpPassword = emailSetting.FirstOrDefault(k => k.Key == "smtpPassword")?.Value,
                    SmtpPort = emailSetting.FirstOrDefault(k => k.Key == "smtpPortNumber")?.Value,
                    SmtpUsername = emailSetting.FirstOrDefault(k => k.Key == "smtpUserName")?.Value
                };
                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SMTP_PasswordEncrypt: {appSetting.SmtpPassword}");

                appSetting.SmtpPassword = _encryptor.Decrypt(appSetting.SmtpPassword);

                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SMTP_PasswordDecrypt: {appSetting.SmtpPassword}");

                var email = new MailMessage(appSetting.SmtpUsername, toEmail);

                var template = await _appDbContext.Templates.Where(x => x.SysName == "ApplicationSubmittedBatch").FirstOrDefaultAsync();
                var templateInfo = await _appDbContext.TemplateInfos.Include(k => k.EmailHeaderAndFooterTemplate).Where(x => x.TemplateId == template.Id).FirstOrDefaultAsync();

                //var otp = GenerateOtp();

                var body = GetProgramSubmissionEnglishContent(userName, programName, templateInfo, email);


                email.Body = body;
                email.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    smtp.Host = appSetting.SmtpHost;
                    smtp.EnableSsl = true;
                    NetworkCredential networkCred = new NetworkCredential(appSetting.SmtpUsername, appSetting.SmtpPassword);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = networkCred;
                    smtp.Port = int.Parse(appSetting.SmtpPort);
                    smtp.Send(email);
                }
                emailResponse.ID = Guid.NewGuid();
                emailResponse.Result = true;
                emailResponse.Message = "Email Sent";
                //emailResponse.OTP = otp;

                return emailResponse;
            }
            catch (Exception e)
            {
                logger.Error(e);
                emailResponse.ID = Guid.NewGuid();
                emailResponse.Result = false;
                emailResponse.Message = "Email Not Sent";
                logger.Info(emailResponse);
                return emailResponse;
            }
        }

        public async Task<EmailView> SendReminderEmailAsync(string toEmail, string content, string userName, int daysLeft, DateTime? registrationEndDate)
        {
            EmailView emailResponse = new EmailView();
            try
            {
                var emailSetting = await _appDbContext.ApplicationSettings.ToListAsync();

                var appSetting = new AppSettings()
                {
                    SmtpHost = emailSetting.FirstOrDefault(k => k.Key == "smtpHostName")?.Value,
                    SmtpPassword = emailSetting.FirstOrDefault(k => k.Key == "smtpPassword")?.Value,
                    SmtpPort = emailSetting.FirstOrDefault(k => k.Key == "smtpPortNumber")?.Value,
                    SmtpUsername = emailSetting.FirstOrDefault(k => k.Key == "smtpUserName")?.Value
                };
                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SMTP_PasswordEncrypt: {appSetting.SmtpPassword}");

                appSetting.SmtpPassword = _encryptor.Decrypt(appSetting.SmtpPassword);

                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SMTP_PasswordDecrypt: {appSetting.SmtpPassword}");

                var email = new MailMessage(appSetting.SmtpUsername, toEmail);

                var template = await _appDbContext.Templates.Where(x => x.SysName == "ProgramActivityReminderTemplateMobile").FirstOrDefaultAsync();
                var templateInfo = await _appDbContext.TemplateInfos.Include(k => k.EmailHeaderAndFooterTemplate).Where(x => x.TemplateId == template.Id).FirstOrDefaultAsync();

                var body = GetReminderContent(content, templateInfo, email, userName, daysLeft, registrationEndDate);


                email.Body = body;
                email.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    smtp.Host = appSetting.SmtpHost;
                    smtp.EnableSsl = true;
                    NetworkCredential networkCred = new NetworkCredential(appSetting.SmtpUsername, appSetting.SmtpPassword);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = networkCred;
                    smtp.Port = int.Parse(appSetting.SmtpPort);
                    smtp.Send(email);
                }
                emailResponse.ID = Guid.NewGuid();
                emailResponse.Result = true;
                emailResponse.Message = "Email Sent";

                return emailResponse;
            }
            catch (Exception e)
            {
                logger.Error(e);
                emailResponse.ID = Guid.NewGuid();
                emailResponse.Result = false;
                emailResponse.Message = "Email Not Sent";
                logger.Info(emailResponse);
                return emailResponse;
            }
        }



        public async Task<EmailView> SendRequestCallbackEmailAsync(string name, string mailAddress, string contact, string fromEmail, string userName)
        {
            EmailView emailResponse = new EmailView();
            try
            {
                var emailSetting = await _appDbContext.ApplicationSettings.ToListAsync();

                var appSetting = new AppSettings()
                {
                    SmtpHost = emailSetting.FirstOrDefault(k => k.Key == "smtpHostName")?.Value,
                    SmtpPassword = emailSetting.FirstOrDefault(k => k.Key == "smtpPassword")?.Value,
                    SmtpPort = emailSetting.FirstOrDefault(k => k.Key == "smtpPortNumber")?.Value,
                    SmtpUsername = emailSetting.FirstOrDefault(k => k.Key == "smtpUserName")?.Value
                };
                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SMTP_PasswordEncrypt: {appSetting.SmtpPassword}");

                appSetting.SmtpPassword = _encryptor.Decrypt(appSetting.SmtpPassword);

                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SMTP_PasswordDecrypt: {appSetting.SmtpPassword}");

                fromEmail = fromEmail != null ? fromEmail : appSetting.SmtpUsername;
                var email = new MailMessage(fromEmail, appSetting.SmtpUsername);
                var template = await _appDbContext.Templates.Where(x => x.SysName == "RequestCallBackMobile").FirstOrDefaultAsync();
                var templateInfo = await _appDbContext.TemplateInfos.Include(k => k.EmailHeaderAndFooterTemplate).Where(x => x.TemplateId == template.Id).FirstOrDefaultAsync();


                var body = GetRequestCallbackContent(name, templateInfo, email, mailAddress, contact, userName);


                email.Body = body;
                email.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    smtp.Host = appSetting.SmtpHost;
                    smtp.EnableSsl = true;
                    NetworkCredential networkCred = new NetworkCredential(appSetting.SmtpUsername, appSetting.SmtpPassword);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = networkCred;
                    smtp.Port = int.Parse(appSetting.SmtpPort);
                    smtp.Send(email);
                }
                emailResponse.ID = Guid.NewGuid();
                emailResponse.Result = true;
                emailResponse.Message = "Email Sent";

                return emailResponse;
            }
            catch (Exception e)
            {
                logger.Error(e);
                emailResponse.ID = Guid.NewGuid();
                emailResponse.Result = false;
                emailResponse.Message = "Email Not Sent";
                logger.Info(emailResponse);
                return emailResponse;
            }
        }

        public async Task<EmailView> SendReportProblemEmailAsync(string reportText, string fromEmail, string userName)
        {
            EmailView emailResponse = new EmailView();
            try
            {
                var emailSetting = await _appDbContext.ApplicationSettings.ToListAsync();

                var appSetting = new AppSettings()
                {
                    SmtpHost = emailSetting.FirstOrDefault(k => k.Key == "smtpHostName")?.Value,
                    SmtpPassword = emailSetting.FirstOrDefault(k => k.Key == "smtpPassword")?.Value,
                    SmtpPort = emailSetting.FirstOrDefault(k => k.Key == "smtpPortNumber")?.Value,
                    SmtpUsername = emailSetting.FirstOrDefault(k => k.Key == "emailUs")?.Value
                };
                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SMTP_PasswordEncrypt: {appSetting.SmtpPassword}");

                appSetting.SmtpPassword = _encryptor.Decrypt(appSetting.SmtpPassword);
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  ToEmail: {appSetting.SmtpUsername}");
                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SMTP_PasswordDecrypt: {appSetting.SmtpPassword}");


                fromEmail = fromEmail != null ? fromEmail : appSetting.SmtpUsername;
                var email = new MailMessage(fromEmail, appSetting.SmtpUsername);
                var template = await _appDbContext.Templates.Where(x => x.SysName == "ProfileCompletenessTemplate").FirstOrDefaultAsync();
                var templateInfo = await _appDbContext.TemplateInfos.Include(k => k.EmailHeaderAndFooterTemplate).Where(x => x.TemplateId == template.Id).FirstOrDefaultAsync();

                var body = GetReportProblemContent(templateInfo, reportText, email, userName);


                email.Body = body;
                email.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    smtp.Host = appSetting.SmtpHost;
                    smtp.EnableSsl = true;
                    NetworkCredential networkCred = new NetworkCredential(appSetting.SmtpUsername, appSetting.SmtpPassword);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = networkCred;
                    smtp.Port = int.Parse(appSetting.SmtpPort);
                    smtp.Send(email);
                }
                emailResponse.ID = Guid.NewGuid();
                emailResponse.Result = true;
                emailResponse.Message = "Email Sent";

                return emailResponse;
            }
            catch (Exception e)
            {
                logger.Error(e);
                emailResponse.ID = Guid.NewGuid();
                emailResponse.Result = false;
                emailResponse.Message = "Email Not Sent";
                logger.Info(emailResponse);
                return emailResponse;
            }
        }

        public async Task<EmailView> SendAssessmentReminderAsync(string toEmail, string content, string userName)
        {
            EmailView emailResponse = new EmailView();
            try
            {
                var emailSetting = await _appDbContext.ApplicationSettings.ToListAsync();

                var appSetting = new AppSettings()
                {
                    SmtpHost = emailSetting.FirstOrDefault(k => k.Key == "smtpHostName")?.Value,
                    SmtpPassword = emailSetting.FirstOrDefault(k => k.Key == "smtpPassword")?.Value,
                    SmtpPort = emailSetting.FirstOrDefault(k => k.Key == "smtpPortNumber")?.Value,
                    SmtpUsername = emailSetting.FirstOrDefault(k => k.Key == "smtpUserName")?.Value
                };
                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SMTP_PasswordEncrypt: {appSetting.SmtpPassword}");

                appSetting.SmtpPassword = _encryptor.Decrypt(appSetting.SmtpPassword);

                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SMTP_PasswordDecrypt: {appSetting.SmtpPassword}");

                var email = new MailMessage(appSetting.SmtpUsername, toEmail);

                var template = await _appDbContext.Templates.Where(x => x.SysName == "AddedAsAssessmentGroupMemberTemplate").FirstOrDefaultAsync();
                var templateInfo = await _appDbContext.TemplateInfos.Include(k => k.EmailHeaderAndFooterTemplate).Where(x => x.TemplateId == template.Id).FirstOrDefaultAsync();

                var body = GetAssessmentContent(content, templateInfo, email, userName);


                email.Body = body;
                email.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    smtp.Host = appSetting.SmtpHost;
                    smtp.EnableSsl = true;
                    NetworkCredential networkCred = new NetworkCredential(appSetting.SmtpUsername, appSetting.SmtpPassword);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = networkCred;
                    smtp.Port = int.Parse(appSetting.SmtpPort);
                    smtp.Send(email);
                }
                emailResponse.ID = Guid.NewGuid();
                emailResponse.Result = true;
                emailResponse.Message = "Email Sent";

                return emailResponse;
            }
            catch (Exception e)
            {
                logger.Error(e);
                emailResponse.ID = Guid.NewGuid();
                emailResponse.Result = false;
                emailResponse.Message = "Email Not Sent";
                logger.Info(emailResponse);
                return emailResponse;
            }
        }

        public async Task<EmailView> SendAssessmentCoordinatorReminderAsync(string toEmail, string content, string userName)
        {
            EmailView emailResponse = new EmailView();
            try
            {
                var emailSetting = await _appDbContext.ApplicationSettings.ToListAsync();

                var appSetting = new AppSettings()
                {
                    SmtpHost = emailSetting.FirstOrDefault(k => k.Key == "smtpHostName")?.Value,
                    SmtpPassword = emailSetting.FirstOrDefault(k => k.Key == "smtpPassword")?.Value,
                    SmtpPort = emailSetting.FirstOrDefault(k => k.Key == "smtpPortNumber")?.Value,
                    SmtpUsername = emailSetting.FirstOrDefault(k => k.Key == "smtpUserName")?.Value
                };
                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SMTP_PasswordEncrypt: {appSetting.SmtpPassword}");

                appSetting.SmtpPassword = _encryptor.Decrypt(appSetting.SmtpPassword);

                //logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SMTP_PasswordDecrypt: {appSetting.SmtpPassword}");

                var email = new MailMessage(appSetting.SmtpUsername, toEmail);

                var template = await _appDbContext.Templates.Where(x => x.SysName == "AddedAsCoordinatorTemplate").FirstOrDefaultAsync();
                var templateInfo = await _appDbContext.TemplateInfos.Include(k => k.EmailHeaderAndFooterTemplate).Where(x => x.TemplateId == template.Id).FirstOrDefaultAsync();

                var body = GetAssessmentCoordinatorContent(content, templateInfo, email, userName);


                email.Body = body;
                email.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    smtp.Host = appSetting.SmtpHost;
                    smtp.EnableSsl = true;
                    NetworkCredential networkCred = new NetworkCredential(appSetting.SmtpUsername, appSetting.SmtpPassword);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = networkCred;
                    smtp.Port = int.Parse(appSetting.SmtpPort);
                    smtp.Send(email);
                }
                emailResponse.ID = Guid.NewGuid();
                emailResponse.Result = true;
                emailResponse.Message = "Email Sent";

                return emailResponse;
            }
            catch (Exception e)
            {
                logger.Error(e);
                emailResponse.ID = Guid.NewGuid();
                emailResponse.Result = false;
                emailResponse.Message = "Email Not Sent";
                logger.Info(emailResponse);
                return emailResponse;
            }
        }
        private static string GetReportProblemContent(TemplateInfo templateInfo, string content, MailMessage email, string userName)
        {
            var body = templateInfo.EmailBodyEn;
            body = body.Replace("Hi #NameEN#,", "Dear Team,<br>");
            body = body.Replace("Seems like we are missing a few details in your profile. Please help us complete your profile", content);
            body = body.Replace("UAE Government Leaders Programme", userName);
            body =
                $@"<div {templateInfo.EmailHtmlBodyTagAttributesEn} > {templateInfo.EmailHeaderAndFooterTemplate.HeaderEn} {body} {templateInfo.EmailHeaderAndFooterTemplate.FooterEn} </div>";
            email.Subject = "Report Problem";
            return body;
        }

        private static string GetReminderContent(string content, TemplateInfo templateInfo,MailMessage email, string userName, int daysLeft, DateTime? registrationEndDate)
        {
            var body = templateInfo.EmailBodyEn;
            body = body.Replace("#Name#", userName);
            body = body.Replace("#Program/Activity#", content);
            body = body.Replace("#date time#", registrationEndDate.ToString());
            body =
                $@"<div {templateInfo.EmailHtmlBodyTagAttributesEn} > {templateInfo.EmailHeaderAndFooterTemplate.HeaderEn} {body} {templateInfo.EmailHeaderAndFooterTemplate.FooterEn} </div>";
            email.Subject = templateInfo.EmailSubjectEn;
            email.Subject = email.Subject.Replace("#Program name#", content);
            return body;
        }

        private static string GetAssessmentContent(string content, TemplateInfo templateInfo, MailMessage email, string userName)
        {
            var body = templateInfo.EmailBodyEn;
            body = body.Replace("#NameEN#", userName);
            body = body.Replace("#GroupName#", content);
            body =
                $@"<div {templateInfo.EmailHtmlBodyTagAttributesEn} > {templateInfo.EmailHeaderAndFooterTemplate.HeaderEn} {body} {templateInfo.EmailHeaderAndFooterTemplate.FooterEn} </div>";
            email.Subject = templateInfo.EmailSubjectEn;
            return body;
        }

        private static string GetAssessmentCoordinatorContent(string content, TemplateInfo templateInfo, MailMessage email, string userName)
        {
            var body = templateInfo.EmailBodyEn;
            body = body.Replace("#NameEN#", userName);
            body = body.Replace("#GroupName#", content);
            body =
                $@"<div {templateInfo.EmailHtmlBodyTagAttributesEn} > {templateInfo.EmailHeaderAndFooterTemplate.HeaderEn} {body} {templateInfo.EmailHeaderAndFooterTemplate.FooterEn} </div>";
            email.Subject = templateInfo.EmailSubjectEn;
            return body;
        }

        private static string GetRequestCallbackContent(string name, TemplateInfo templateInfo,MailMessage email, string emailAddress, string contact, string userName)
        {
            var body = templateInfo.EmailBodyEn;
            body = body.Replace("#Name#" , name);
            body = body.Replace("#EmailAddress#", emailAddress);
            body = body.Replace("#Contact#", contact);
            body = body.Replace("#UserName#", userName);
            body =
                $@"<div {templateInfo.EmailHtmlBodyTagAttributesEn} > {templateInfo.EmailHeaderAndFooterTemplate.HeaderEn} {body} {templateInfo.EmailHeaderAndFooterTemplate.FooterEn} </div>";
            email.Subject = templateInfo.EmailSubjectEn;
            return body;
        }
        private static string GetActivationEnglishContent(string userName, TemplateInfo templateInfo, int otp, MailMessage email)
        {
            var body = templateInfo.EmailBodyEn;
            body = body.Replace("#NameEN#", userName);
            body = body.Replace("#OTP#", otp.ToString());
            body =
                $@"<div {templateInfo.EmailHtmlBodyTagAttributesEn} > {templateInfo.EmailHeaderAndFooterTemplate.HeaderEn} {body} {templateInfo.EmailHeaderAndFooterTemplate.FooterEn} </div>";
            email.Subject = templateInfo.EmailSubjectEn;
            return body;
        }

        private static string GetProgramSubmissionEnglishContent(string userName, string programName, TemplateInfo templateInfo, MailMessage email)
        {
            var body = templateInfo.EmailBodyEn;
            body = body.Replace("#NameEN#", userName);
            body = body.Replace("#ProgrammeName#", programName);
            body =
                $@"<div {templateInfo.EmailHtmlBodyTagAttributesEn} > {templateInfo.EmailHeaderAndFooterTemplate.HeaderEn} {body} {templateInfo.EmailHeaderAndFooterTemplate.FooterEn} </div>";
            email.Subject = templateInfo.EmailSubjectEn;
            return body;
        }



        private static string GetActivationArabicContent(string userName, TemplateInfo templateInfo, int otp, MailMessage email)
        {
            var body = templateInfo.EmailBodyAr;
            body = body.Replace("#NameAR#", userName);
            body = body.Replace("#OTP#", otp.ToString());
            body =
                $@"<div {templateInfo.EmailHtmlBodyTagAttributesAr} > {templateInfo.EmailHeaderAndFooterTemplate.HeaderAr} {body} {templateInfo.EmailHeaderAndFooterTemplate.FooterAr} </div>";
            email.Subject = templateInfo.EmailSubjectAr;
            return body;
        }



        private static string GetResetPasswordEnglishContent(string userName, TemplateInfo templateInfo, int otp, MailMessage email)
        {
            var body = templateInfo.EmailBodyEn;
            body = body.Replace("#NameEN#", userName);
            body = body.Replace("#OTP#", otp.ToString());
           
            body =
                $@"<div {templateInfo.EmailHtmlBodyTagAttributesEn} > {templateInfo.EmailHeaderAndFooterTemplate.HeaderEn} {body} {templateInfo.EmailHeaderAndFooterTemplate.FooterEn} </div>";
            email.Subject = templateInfo.EmailSubjectEn;
            return body;
        }

        private static string GetResetPasswordArabicContent(string userName, TemplateInfo templateInfo, int otp, MailMessage email)
        {
            var body = templateInfo.EmailBodyAr;
            body = body.Replace("#NameAR#", userName);
            body = body.Replace("#OTP#", otp.ToString());
          
            body =
                $@"<div {templateInfo.EmailHtmlBodyTagAttributesAr} > {templateInfo.EmailHeaderAndFooterTemplate.HeaderAr} {body} {templateInfo.EmailHeaderAndFooterTemplate.FooterAr} </div>";
            email.Subject = templateInfo.EmailSubjectAr;
            return body;
        }

        public int GenerateOtp()
        {
            return _random.Next(1000, 9999);
        }
    }
}
