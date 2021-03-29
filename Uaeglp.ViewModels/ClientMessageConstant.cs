using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.ViewModels
{
    public class ClientMessageConstant
    {
        public static string ProfileNotExist = ((int)ExceptionEnumType.ProfileNotExist).ToString();
        public static string WeAreUnableToProcessYourRequest = ((int)ExceptionEnumType.InternalServerError).ToString();
        public static string FileNotFound = ((int)ExceptionEnumType.FileNotFound).ToString();
        public static string ImagePostNotExist = ((int)ExceptionEnumType.ImagePostNotExist).ToString();
        public static string PollNotFound = ((int)ExceptionEnumType.PollNotFound).ToString();
        public static string PostNotFound = ((int)ExceptionEnumType.PostNotFound).ToString();
        public static string UserNotFound = ((int)ExceptionEnumType.UserNotFound).ToString();
        public static string Success = ((int)ExceptionEnumType.Success).ToString();
        public static string Deeplink = ((int)ExceptionEnumType.Deeplink).ToString();
        public static string EmailIdExists = ((int)ExceptionEnumType.EmailIdExists).ToString();
        public static string EmiratesIdExist = ((int)ExceptionEnumType.EmiratesIdExist).ToString();
        public static string PasswordIsWrong = ((int)ExceptionEnumType.PasswordIsWrong).ToString();
        public static string OTPExpiredOrInvalid = ((int)ExceptionEnumType.OTPExpiredOrInvalid).ToString();
        public static string NotRegisteredEmiratesId = ((int)ExceptionEnumType.NotRegisteredEmiratesId).ToString();
        public static string NotRegisteredEmailId = ((int)ExceptionEnumType.NotRegisteredEmailId).ToString();
        public static string InvalidUserNameAndPassword = ((int)ExceptionEnumType.InvalidUserNameAndPassword).ToString();
        public static string PasswordUpdatedSuccessfully = ((int)ExceptionEnumType.PasswordUpdatedSuccessfully).ToString();
        public static string EIDNotEligible = ((int)ExceptionEnumType.EIDNotEligible).ToString();
        public static string EIDCitizen = ((int)ExceptionEnumType.EIDCitizen).ToString();
        public static string EIDNotFound = ((int)ExceptionEnumType.EIDNotFound).ToString();
        public static string EIDAPINotWorking = ((int)ExceptionEnumType.EIDAPINotWorking).ToString();
        public static string EIDExist = ((int)ExceptionEnumType.EIDExist).ToString();
        public static string AlreadyJoined = ((int)ExceptionEnumType.AlreadyJoined).ToString();
        public static string PastEvent = ((int)ExceptionEnumType.PastEvent).ToString();
        public static string FutureEvent = ((int)ExceptionEnumType.FutureEvent).ToString();
        public static string InvalidQRCode = ((int)ExceptionEnumType.InvalidQRCode).ToString();
        public static string InvalidShortCode = ((int)ExceptionEnumType.InvalidShortCode).ToString();
        public static string NotEligible = ((int)ExceptionEnumType.NotEligible).ToString();
    }
}
