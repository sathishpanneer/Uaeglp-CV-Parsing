using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Uaeglp.Models
{
    [Table("QuestionAnswer_Option")]
    public partial class QuestionAnswerOption
    {
        [Key]
        [Column("QuestionAnswerID")]
        public int QuestionanswerID { get; set; }
        [Key]
        [Column("OptionID")]
        public int optionID { get; set; }

        [ForeignKey(nameof(QuestionanswerID))]
        //[InverseProperty("Questionansweroptions")]
        public virtual QuestionAnswer Questionanswer { get; set; }

        [ForeignKey(nameof(optionID))]
        //[InverseProperty("Questionansweroptions")]
        public virtual Option option { get; set; }
    }
}
