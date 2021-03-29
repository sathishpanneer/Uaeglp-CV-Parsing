using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.Contracts
{
    public interface IEmailService
    {
        Task<EmailView> SendActivationEmailAsync(string toEmail, string userName,
            LanguageType languageType = LanguageType.EN);

        Task<EmailView> SendResetPasswordEmailAsync(string toEmail, string userName,
            LanguageType languageType = LanguageType.EN);
        Task<EmailView> SendReminderEmailAsync(string toEmail, string content, string userName, int daysLeft, DateTime? registrationEndDate);
        Task<EmailView> SendAssessmentReminderAsync(string toEmail, string content, string userName);
        Task<EmailView> SendAssessmentCoordinatorReminderAsync(string toEmail, string content, string userName);
        Task<EmailView> SendRequestCallbackEmailAsync(string name, string mailAddress, string contact, string email, string userName);
        Task<EmailView> SendReportProblemEmailAsync(string reportText, string email, string userName);
        Task<EmailView> SendProgramSubmissionEmailAsync(string toEmail, string programName, string userName);
        int GenerateOtp();
    }
}
