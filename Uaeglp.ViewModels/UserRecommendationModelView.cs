using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels
{
    public class UserRecommendationModelView
    {
        //public int ID { get; set; }
        public int SenderUserID { get; set; }
        public int RecipientUserID { get; set; }
        public string RecommendationText { get; set; }
       // public bool isAccepted { get; set; }
        public bool isAskedRecommendation { get; set; }
        public bool isRead { get; set; }
        public PublicProfileView SenderInfo { get; set; }
    }
}
