using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Uaeglp.ViewModels;

namespace Uaeglp.Models
{
    [Table("RecommendationSubmissions", Schema = "dbo")]
    public class RecommendLeadr
    {
        [Key]
        public int ID { get; set; }
        [StringLength(50)]
        public string FullName { get; set; }
        [StringLength(2550)]
        public string RecommendingText { get; set; }
        public Guid? RecommendingAudioID { get; set; }
        public Guid? RecommendingVideoID { get; set; }
        [StringLength(30)]
        public string Occupation { get; set; }
        [StringLength(255)]
        public string ContactNumber { get; set; }
        [StringLength(256)]
        public string Email { get; set; }
        [StringLength(2500)]
        public string LinkedinURL { get; set; }
        [StringLength(2500)]
        public string TwitterURL { get; set; }
        [StringLength(2500)]
        public string InstagramURL { get; set; }
        public int? RecommendedProfileID { get; set; }
        public int? RecommenderProfileID { get; set; }
        public int? SourceItemID { get; set; }
        public int? StatusItemID { get; set; }
        public string OtherFitment { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }


        [ForeignKey(nameof(SourceItemID))]
        [InverseProperty(nameof(LookupItem.RecommendationSourceItem))]
        public virtual LookupItem LookupRecommendSourceFit { get; set; }

        [ForeignKey(nameof(StatusItemID))]
        [InverseProperty(nameof(LookupItem.RecommendationStatusItem))]
        public virtual LookupItem LookupRecommendStatusItem { get; set; }

        [ForeignKey(nameof(RecommendedProfileID))]
        [InverseProperty(nameof(LookupItem.RecommendationProfileRecommended))]
        public virtual LookupItem LookupRecommendProfileRecommended { get; set; }

        [ForeignKey(nameof(RecommenderProfileID))]
        [InverseProperty(nameof(LookupItem.RecommendationProfileRecommender))]
        public virtual LookupItem LookupRecommendProfileRecommender { get; set; }

        [InverseProperty(nameof(RecommendationFitDetails.RecommendLeadr))]
        public virtual ICollection<RecommendationFitDetails> RecommendationFitDetail { get; set; }

        [InverseProperty(nameof(RecommendationCallback.RecommendLeadr))]
        public virtual ICollection<RecommendationCallback> RecommendationCallbacks { get; set; }

    }
}
