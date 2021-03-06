// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("VideoAssessment")]
    public partial class VideoAssessment
    {
        public VideoAssessment()
        {
            Batches = new HashSet<Batch>();
            ProfileVideoAssessmentAnswerScores = new HashSet<ProfileVideoAssessmentAnswerScore>();
            ProfileVideoAssessmentCriteriaScores = new HashSet<ProfileVideoAssessmentCriteriaScore>();
            ProfileVideoAssessmentScores = new HashSet<ProfileVideoAssessmentScore>();
            VideoAssessmentQuestions = new HashSet<VideoAssessmentQuestion>();
        }

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
        [Required]
        [Column("NameEN")]
        [StringLength(100)]
        public string NameEn { get; set; }
        [Required]
        [Column("NameAR")]
        [StringLength(100)]
        public string NameAr { get; set; }
        [Column("DescriptionEN")]
        public string DescriptionEn { get; set; }
        [Column("DescriptionAR")]
        public string DescriptionAr { get; set; }
        [Column("CoverPageEN")]
        public string CoverPageEn { get; set; }
        [Column("CoverPageAR")]
        public string CoverPageAr { get; set; }
        [Column("CriteriaID")]
        public int CriteriaId { get; set; }

        [ForeignKey(nameof(CriteriaId))]
        [InverseProperty(nameof(VidAssesCriterion.VideoAssessments))]
        public virtual VidAssesCriterion Criteria { get; set; }
        [InverseProperty(nameof(Batch.VideoAssessment))]
        public virtual ICollection<Batch> Batches { get; set; }
        [InverseProperty(nameof(ProfileVideoAssessmentAnswerScore.VideoAssessment))]
        public virtual ICollection<ProfileVideoAssessmentAnswerScore> ProfileVideoAssessmentAnswerScores { get; set; }
        [InverseProperty(nameof(ProfileVideoAssessmentCriteriaScore.VideoAssessment))]
        public virtual ICollection<ProfileVideoAssessmentCriteriaScore> ProfileVideoAssessmentCriteriaScores { get; set; }
        [InverseProperty(nameof(ProfileVideoAssessmentScore.VideoAssessment))]
        public virtual ICollection<ProfileVideoAssessmentScore> ProfileVideoAssessmentScores { get; set; }
        [InverseProperty(nameof(VideoAssessmentQuestion.VideoAssessment))]
        public virtual ICollection<VideoAssessmentQuestion> VideoAssessmentQuestions { get; set; }
    }
}