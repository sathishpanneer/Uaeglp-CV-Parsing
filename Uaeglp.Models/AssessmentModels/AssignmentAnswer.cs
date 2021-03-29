﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("AssignmentAnswer")]
    public partial class AssignmentAnswer
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("ProfileID")]
        public int ProfileId { get; set; }
        [Column("AssighmentID")]
        public int AssighmentId { get; set; }
        public string Answer { get; set; }
        [Column("StatusItemID")]
        public int StatusItemId { get; set; }
        [StringLength(4000)]
        public string ReworkComment { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SubmitDate { get; set; }
        [Column("CorrelationID")]
        public Guid? CorrelationId { get; set; }
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

        [ForeignKey(nameof(AssighmentId))]
        [InverseProperty(nameof(Assignment.AssignmentAnswers))]
        public virtual Assignment Assighment { get; set; }
        [ForeignKey(nameof(ProfileId))]
        [InverseProperty("AssignmentAnswers")]
        public virtual Profile Profile { get; set; }
        [ForeignKey(nameof(StatusItemId))]
        [InverseProperty(nameof(LookupItem.AssignmentAnswers))]
        public virtual LookupItem StatusItem { get; set; }
    }
}