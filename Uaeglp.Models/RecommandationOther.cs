using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Uaeglp.Models
{
    [Table("RecommandationOthers", Schema = "app")]
    public class RecommandationOther
    {
        [Key]
        public int ID { get; set; }
        public int RecommendID { get; set; }
        public string OtherFit { get; set; }
       /// [ForeignKey(nameof(RecommendID))]
        //[InverseProperty("RecommandationOthers")]
        //public virtual RecommendLeadr RecommendLeadr { get; set; }
    }
}
