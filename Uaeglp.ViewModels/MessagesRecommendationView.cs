using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class MessagesRecommendationView
    {
        public RoomViewModel UserMessage { get; set; }
        public UserRecommendView UserRecommend { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
