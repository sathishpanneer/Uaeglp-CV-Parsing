using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.ProfileViewModels
{
    public class CriteriaClaimedPointsView
    {
        public int ProfileId { get; set; }
        public int CriteriaId { get; set; }
        public int TotalClaimedPoints { get; set; }
        public int CriteriaClaimedPoints { get; set; }
        public List<CriteriaClaimView> ClaimedList { get; set; }
        public List<CriteriaClaimView> PendingList { get; set; }


    }
}
