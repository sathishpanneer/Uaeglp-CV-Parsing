using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class PushNotificationModel
    {
        public string to { get; set; }
        public FcmNotification notification { get; set; }
        public object data { get; set; }
    }
    public class FcmNotification
    {
        public string title { get; set; }
        public string text { get; set; }
        public string body { get; set; }
        public string sound { get; set; }
    }
}
