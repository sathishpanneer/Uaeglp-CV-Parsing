using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels
{
    public class LoginResponseViewModel
    {
       
        public string Token { get; set; }

        public bool IsAuthorized { get; set; }

        public UserDetailsView UserDetails { get; set; }
        public bool  Success { get; set; }
        public string  Message { get; set; }
        public int ExpireMinutes { get; set; }
    }
}
