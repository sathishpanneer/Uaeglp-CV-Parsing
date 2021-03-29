using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Uaeglp.Models
{
    [Table("RecommendationCallbacks", Schema = "dbo")]
    public class RecommendationCallback
    {
        [Key]
        public int ID { get; set; }
        public int? RecommendID { get; set; }
        public string FullName { get; set; }
        public string ContactNumber { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string Email { get; set; }
        [ForeignKey(nameof(RecommendID))]
        [InverseProperty("RecommendationCallbacks")]
        public virtual RecommendLeadr RecommendLeadr { get; set; }
    }
}
