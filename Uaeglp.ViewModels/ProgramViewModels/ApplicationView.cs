using System;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels.ProgramViewModels
{
    public class ApplicationView
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public int ProfileId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public int? StatusItemId { get; set; }
        public int? ReviewerId { get; set; }
        public int? SecurityItemId { get; set; }
        public int? ReviewStatusItemId { get; set; }
        public int? AssessmentItemId { get; set; }
        public decimal TotalScore { get; set; }
        public int CompletionPercentage { get; set; }
        public DateTime? SubmitTime { get; set; }
        public int? VideoAssessmentStatusId { get; set; }
        public int? BatchRank { get; set; }
        public decimal? TotalAssessmentScore { get; set; }
        public bool IsRecordUpdated { get; set; }
        public bool? IsRouted { get; set; }
        public DateTime? AssessmentCompletionDatetime { get; set; }
        public bool? IsCloned { get; set; }
        public bool IsShortlisted { get; set; }
        public string ReferenceNumber { get; set; }

        public LookupItemView AssessmentItem { get; set; }
        public BatchView Batch { get; set; }
        public LookupItemView ReviewStatusItem { get; set; }
        public LookupItemView SecurityItem { get; set; }
        public LookupItemView StatusItem { get; set; }
        public LookupItemView VideoAssessmentStatus { get; set; }
    }
}
