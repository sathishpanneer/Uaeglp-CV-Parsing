using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public enum CustomStatusCode
        {
            Success,
            EmiratesIDExist,
            InvalidEmiratesID,
            EmailIDExists,
            FunctionalError,
            OTPExpiredOrInvalid,
            UserNotExist,
            NotRegisteredEmailID,
            PasswordIsWrong,
            NotRegisteredEmiratesID, 
            Citizen,
            Expat,
            NotFound,
            APINotWorking,
            EIDExist

    }
    
}
