﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("AssessmentTool")]
    public partial class AssessmentTool
    {
        public AssessmentTool()
        {
            AssessmentBlocks = new HashSet<AssessmentBlock>();
            BatchAssessmentTools = new HashSet<BatchAssessmentTool>();
            CompetencyAssessmentTools = new HashSet<CompetencyAssessmentTool>();
            Factors = new HashSet<Factor>();
            MatricesTool = new HashSet<MatrixTool>();
            PillarAssessmentTools = new HashSet<PillarAssessmentTool>();
            ProfileAssessmentToolScores = new HashSet<ProfileAssessmentToolScore>();
            ProfileCompetencyScores = new HashSet<ProfileCompetencyScore>();
            ProfilePillarScores = new HashSet<ProfilePillarScore>();
            ProfileQuestionItemScores = new HashSet<ProfileQuestionItemScore>();
            ProfileSubAssessmentToolScores = new HashSet<ProfileSubAssessmentToolScore>();
            QuestionHeads = new HashSet<QuestionHead>();
            QuestionItems = new HashSet<QuestionItem>();
            SubAssessmentTools = new HashSet<SubAssessmentTool>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [Column("NameEN")]
        [StringLength(100)]
        public string NameEn { get; set; }
        [Required]
        [Column("NameAR")]
        [StringLength(100)]
        public string NameAr { get; set; }
        [Column("ImageID")]
        public Guid ImageId { get; set; }
        [Column("MobImageID")]
        public Guid? MobImageId { get; set; }
        [Column("DescriptionEN")]
        public string DescriptionEn { get; set; }
        [Column("DescriptionAR")]
        public string DescriptionAr { get; set; }
        public int ValidityRangeNumber { get; set; }
        public int? AssessmentToolType { get; set; }
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
        public bool HasSubscale { get; set; }
        public bool HasQuestionDirect { get; set; }
        [Column("CoverPageEN")]
        public string CoverPageEn { get; set; }
        [Column("CoverPageAR")]
        public string CoverPageAr { get; set; }
        public bool HasQuestionHead { get; set; }
        public int AssessmentToolCategory { get; set; }
        public int DisplayOrder { get; set; }
        [Required]
        public bool? IsPublished { get; set; }
        [Column(TypeName = "decimal(20, 10)")]
        public decimal? Mean { get; set; }
        [Column(TypeName = "decimal(20, 10)")]
        public decimal? StandardDeviation { get; set; }
        public bool IsDefaultVisible { get; set; }
        [Column("DurationAR")]
        public string DurationAr { get; set; }
        [Column("DurationEN")]
        public string DurationEn { get; set; }
        [Column("InstructionsEN")]
        public string InstructionsEn { get; set; }
        [Column("InstructionsAR")]
        public string InstructionsAr { get; set; }

        [ForeignKey(nameof(AssessmentToolCategory))]
        [InverseProperty(nameof(LookupItem.AssessmentToolAssessmentToolCategoryNavigations))]
        public virtual LookupItem AssessmentToolCategoryNavigation { get; set; }
        [ForeignKey(nameof(AssessmentToolType))]
        [InverseProperty(nameof(LookupItem.AssessmentToolAssessmentToolTypeNavigations))]
        public virtual LookupItem AssessmentToolTypeNavigation { get; set; }
        [InverseProperty(nameof(AssessmentBlock.Assessmenttool))]
        public virtual ICollection<AssessmentBlock> AssessmentBlocks { get; set; }
        [InverseProperty(nameof(BatchAssessmentTool.AssessmentTool))]
        public virtual ICollection<BatchAssessmentTool> BatchAssessmentTools { get; set; }
        [InverseProperty(nameof(CompetencyAssessmentTool.AssessmentTool))]
        public virtual ICollection<CompetencyAssessmentTool> CompetencyAssessmentTools { get; set; }
        [InverseProperty(nameof(Factor.AssessmentTool))]
        public virtual ICollection<Factor> Factors { get; set; }
        [InverseProperty(nameof(MatrixTool.AssessmentTool))]
        public virtual ICollection<MatrixTool> MatricesTool { get; set; }
        [InverseProperty(nameof(PillarAssessmentTool.AssessmentTool))]
        public virtual ICollection<PillarAssessmentTool> PillarAssessmentTools { get; set; }
        [InverseProperty(nameof(ProfileAssessmentToolScore.AssessmentTool))]
        public virtual ICollection<ProfileAssessmentToolScore> ProfileAssessmentToolScores { get; set; }
        [InverseProperty(nameof(ProfileCompetencyScore.Assessmenttool))]
        public virtual ICollection<ProfileCompetencyScore> ProfileCompetencyScores { get; set; }
        [InverseProperty(nameof(ProfilePillarScore.Assessmenttool))]
        public virtual ICollection<ProfilePillarScore> ProfilePillarScores { get; set; }
        [InverseProperty(nameof(ProfileQuestionItemScore.AssessmentTool))]
        public virtual ICollection<ProfileQuestionItemScore> ProfileQuestionItemScores { get; set; }
        [InverseProperty(nameof(ProfileSubAssessmentToolScore.AssessmentTool))]
        public virtual ICollection<ProfileSubAssessmentToolScore> ProfileSubAssessmentToolScores { get; set; }
        [InverseProperty(nameof(QuestionHead.AssessmentTool))]
        public virtual ICollection<QuestionHead> QuestionHeads { get; set; }
        [InverseProperty(nameof(QuestionItem.AssessmentTool))]
        public virtual ICollection<QuestionItem> QuestionItems { get; set; }
        [InverseProperty(nameof(SubAssessmentTool.AssessmentTool))]
        public virtual ICollection<SubAssessmentTool> SubAssessmentTools { get; set; }
    }
}