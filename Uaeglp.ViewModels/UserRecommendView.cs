using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels
{
    public class UserRecommendView
    {
        public int ID { get; set; }
        public int SenderUserID { get; set; }
        public int RecipientUserID { get; set; }
        public string RecommendationText { get; set; }
        public bool isAccepted { get; set; }
        public bool isDeclined { get; set; }
        public bool isAskedRecommendation { get; set; }
        public bool isRead { get; set; }
  
        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }
        public string? ModifiedBy { get; set; }
        public PublicProfileView UserInfo { get; set; }
    }
}
