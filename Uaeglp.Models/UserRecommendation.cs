using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Uaeglp.Models
{
    [Table("UserRecommendation", Schema ="app")]
    public class UserRecommendation
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int SenderUserID { get; set; }
        [Required]
        public int RecipientUserID { get; set; }
        public string RecommendationText { get; set; }
        public bool isAccepted { get; set; }
        public bool isAskedRecommendation { get; set; }
        public bool isDeclined { get; set; }
        public bool? isRead { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Modified { get; set; }
   
        [StringLength(256)]
        public string? CreatedBy { get; set; }

        [StringLength(256)]
        public string? ModifiedBy { get; set; }

    }
}
