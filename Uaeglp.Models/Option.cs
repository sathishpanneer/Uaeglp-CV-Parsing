﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("Option")]
    public partial class Option
    {
        public Option()
        {
            QuestionAnswers = new HashSet<QuestionAnswer>();
           Questionansweroptions = new HashSet<QuestionAnswerOption>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [Column("TextEN")]
        [StringLength(300)]
        public string TextEn { get; set; }
        [Required]
        [Column("TextAR")]
        [StringLength(300)]
        public string TextAr { get; set; }
        public int? Value { get; set; }
        [Column("QuestionID")]
        public int? QuestionId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; }
        [Required]
        [StringLength(256)]
        public string ModifiedBy { get; set; }

        [ForeignKey(nameof(QuestionId))]
        [InverseProperty("Options")]
        public virtual Question Question { get; set; }
        [InverseProperty(nameof(QuestionAnswer.SelectedOption))]
        public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; }

        //[InverseProperty(nameof(QuestionAnswerOption.optionID))]
        public virtual ICollection<QuestionAnswerOption> Questionansweroptions { get; set; }
    }
}