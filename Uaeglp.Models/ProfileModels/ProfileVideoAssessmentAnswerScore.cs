﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("ProfileVideoAssessmentAnswerScore")]
    public partial class ProfileVideoAssessmentAnswerScore
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
        [Column("QuestionID")]
        public int QuestionId { get; set; }
        [Column("VideoID")]
        public int? VideoId { get; set; }
        [Column("VideoIDYoutubeURL")]
        public string VideoIdyoutubeUrl { get; set; }
        [Column("VideoAssessmentID")]
        public int VideoAssessmentId { get; set; }
        [Column("ProfileVideoAssessmentScoreID")]
        public int ProfileVideoAssessmentScoreId { get; set; }
        public int Order { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Score { get; set; }

        [ForeignKey(nameof(ProfileId))]
        [InverseProperty("ProfileVideoAssessmentAnswerScores")]
        public virtual Profile Profile { get; set; }
        [ForeignKey(nameof(ProfileVideoAssessmentScoreId))]
        [InverseProperty("ProfileVideoAssessmentAnswerScores")]
        public virtual ProfileVideoAssessmentScore ProfileVideoAssessmentScore { get; set; }
        [ForeignKey(nameof(QuestionId))]
        [InverseProperty(nameof(VideoAssessmentQuestion.ProfileVideoAssessmentAnswerScores))]
        public virtual VideoAssessmentQuestion Question { get; set; }
        [ForeignKey(nameof(VideoId))]
        [InverseProperty(nameof(File.ProfileVideoAssessmentAnswerScores))]
        public virtual File Video { get; set; }
        [ForeignKey(nameof(VideoAssessmentId))]
        [InverseProperty("ProfileVideoAssessmentAnswerScores")]
        public virtual VideoAssessment VideoAssessment { get; set; }
    }
}