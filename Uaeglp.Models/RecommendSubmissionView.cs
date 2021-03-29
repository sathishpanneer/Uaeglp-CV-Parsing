using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Utilities;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Models
{
    public class RecommendSubmissionView
    {
        public int? ID { get; set; }
        public string FullName { get; set; }
        public string RecommendingText { get; set; }
        public Guid? RecommendingAudioID { get; set; }
        public Guid? RecommendingVideoID { get; set; }
        public string Occupation { get; set; }
        public string ContactNumber { get; set; }   
        public string Email { get; set; }
        public string LinkedinURL { get; set; }
        public string TwitterURL { get; set; }
        public string InstagramURL { get; set; }
        public int RecommendedProfileID { get; set; }
        public int RecommenderProfileID { get; set; }
        public string OtherFitment { get; set; }
        public string RecommendAudioUrl => RecommendingAudioID != null  ? ConstantUrlPath.AudioDownloadPath + ID : "";
        public string RecommendVideoUrl => RecommendingVideoID != null ? ConstantUrlPath.VideoDownloadPath + ID : "";
        public int? SourceItemID { get; set; }
        public string StatusItemID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public PublicProfileView RecommendedBy { get; set; }
        public string RecommendViewProfileURL { get; set; }
        public int? RecommendViewProfileID { get; set; }
        //public IList<int> RecommendLeaderFit { get; set; }
        //public string OthersText { get; set; }
    }
}
