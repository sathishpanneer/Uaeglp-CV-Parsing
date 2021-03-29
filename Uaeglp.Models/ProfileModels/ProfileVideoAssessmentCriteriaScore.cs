﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("ProfileVideoAssessmentCriteriaScore")]
    public partial class ProfileVideoAssessmentCriteriaScore
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
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
        [Column("ProfileID")]
        public int ProfileId { get; set; }
        public int Order { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Score { get; set; }
        [Column("CriteriaID")]
        public int CriteriaId { get; set; }
        [Column("CriterionID")]
        public int CriterionId { get; set; }
        [Column("VideoAssessmentID")]
        public int VideoAssessmentId { get; set; }
        [Column("ProfileVideoAssessmentScoreID")]
        public int ProfileVideoAssessmentScoreId { get; set; }

        [ForeignKey(nameof(CriteriaId))]
        [InverseProperty(nameof(VidAssesCriterion.ProfileVideoAssessmentCriteriaScores))]
        public virtual VidAssesCriterion Criteria { get; set; }
        [ForeignKey(nameof(CriterionId))]
        [InverseProperty(nameof(Criterion1.ProfileVideoAssessmentCriteriaScores))]
        public virtual Criterion1 Criterion { get; set; }
        [ForeignKey(nameof(ProfileId))]
        [InverseProperty("ProfileVideoAssessmentCriteriaScores")]
        public virtual Profile Profile { get; set; }
        [ForeignKey(nameof(ProfileVideoAssessmentScoreId))]
        [InverseProperty("ProfileVideoAssessmentCriteriaScores")]
        public virtual ProfileVideoAssessmentScore ProfileVideoAssessmentScore { get; set; }
        [ForeignKey(nameof(VideoAssessmentId))]
        [InverseProperty("ProfileVideoAssessmentCriteriaScores")]
        public virtual VideoAssessment VideoAssessment { get; set; }
    }
}