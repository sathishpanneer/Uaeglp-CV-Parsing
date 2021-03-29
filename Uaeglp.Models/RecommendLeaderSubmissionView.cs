using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels;

namespace Uaeglp.Models
{
    public class RecommendLeaderSubmissionView
    {
        public RecommendSubmissionView RecommendLeaderSubmission { get; set; }
        public List<RecommendSubmissionView> RecommendLeaderList { get; set; }
        public List<int?> RecommendLeaderFitment { get; set; }
        public List<RecommendFitListView> RecommendLeaderFitmentDetails { get; set; }
        //public RecommendOthersView RecommendLeaderOther { get; set; }

        public RecommendProfileView RecommendLeaderProfile { get; set; }
    }
}
