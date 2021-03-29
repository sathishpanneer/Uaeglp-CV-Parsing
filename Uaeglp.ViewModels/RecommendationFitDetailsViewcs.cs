using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels
{
    public class RecommendationFitDetailsView
    {
        public List<LookupItemView> RecommendLeaderFitList { get; set; }
        public List<LookupItemView> RecommendLeaderSourceItemList { get; set; }
        public List<LookupItemView> RecommendLeaderStatusItem { get; set; }
    }
}
