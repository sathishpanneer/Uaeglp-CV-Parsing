using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Uaeglp.Models
{
    [Table("RecommendationSubmission_RecommendationFitLookupItems", Schema = "dbo")]
    public class RecommendationFitDetails
    {
        [Key]
        public int RecommendID { get; set; }
        public int? RecommendationFitItemId { get; set; }

        [ForeignKey(nameof(RecommendID))]
        [InverseProperty("RecommendationFitDetail")]
        public virtual RecommendLeadr RecommendLeadr { get; set; }
        [ForeignKey(nameof(RecommendationFitItemId))]
        [InverseProperty(nameof(LookupItem.RecommendationFitItem))]
        public virtual LookupItem LookupRecommendFit { get; set; }
    }
}
