using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class UserCustomNotificationView
    {
        public int ProfileID { get; set; }
        public int CategoryID { get; set; }
        public bool isEnabled { get; set; }
    }
}
