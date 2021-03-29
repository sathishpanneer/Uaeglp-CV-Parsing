using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.Enums
{
    public enum ExceptionEnumType
    {
        Success =200,
        PasswordUpdatedSuccessfully =201,
        EmailIdExists = 300,
        EmiratesIdExist =301,
        PasswordIsWrong =302,
        OTPExpiredOrInvalid = 303,
        NotRegisteredEmiratesId =304,
        NotRegisteredEmailId = 305,
        ProfileNotExist = 404,
        UserNotFound = 405,
        FileNotFound = 407,
        PollNotFound = 408,
        PostNotFound = 409,
        InvalidUserNameAndPassword =410,
        ImagePostNotExist = 411,
        Deeplink = 412,
        InternalServerError = 500,
        EIDCitizen = 92001,
        EIDNotEligible = 92002,
        EIDNotFound = 92003,
        EIDAPINotWorking = 92005,
        EIDExist = 92006,
        AlreadyJoined = 92007,
        PastEvent = 92008,
        FutureEvent = 92009,
        InvalidQRCode = 92010,
        NotEligible = 92011,
        InvalidShortCode = 92012,

    }
}
