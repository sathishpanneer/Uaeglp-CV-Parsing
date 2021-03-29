using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.PushNotificationViewModel
{
    public class PushNotifyResponse
    {
        public long multicast_id { get; set; }
        public int success { get; set; }
        public int failure { get; set; }
        public int canonical_ids { get; set; }
        public List<Results> results { get; set; }
    }

    public class Results
    {
        public string message_id { get; set; }
        public string error { get; set; }
    }
}
