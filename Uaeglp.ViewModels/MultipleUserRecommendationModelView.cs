using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
    public class MultipleUserRecommendationModelView
    {
        public int SenderUserID { get; set; }
        public List<int> RecipientUserID { get; set; }
        public string RecommendationText { get; set; }
        public bool isAskedRecommendation { get; set; }
    }
}
