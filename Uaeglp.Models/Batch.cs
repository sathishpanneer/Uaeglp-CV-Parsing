﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("Batch")]
    public partial class Batch
    {
        public Batch()
        {
            Applications = new HashSet<Application>();
            Assignments = new HashSet<Assignment>();
            BatchAssessmentTools = new HashSet<BatchAssessmentTool>();
            BatchInitiatives = new HashSet<BatchInitiative>();
            Events = new HashSet<Event>();
            FolderBatches = new HashSet<FolderBatch>();
            Glppermissions = new HashSet<Glppermission>();
            ProfileBatchAssessments = new HashSet<ProfileBatchAssessment>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("ProgrammeID")]
        public int ProgrammeId { get; set; }
        public int Number { get; set; }
        public int Year { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateRegFrom { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateFrom { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateRegTo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateTo { get; set; }
        public bool IsClosed { get; set; }
        [Column("QuestionGroupID")]
        public int QuestionGroupId { get; set; }
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
        [Column("AssessmentMatrixID")]
        public int? AssessmentMatrixId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AssessmentStartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AssessmentEndDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? VideoAssessmentStartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? VideoAssessmentEndDate { get; set; }
        [Column("VideoAssessmentID")]
        public int? VideoAssessmentId { get; set; }
        [StringLength(300)]
        public string ContactEmail { get; set; }
        [StringLength(300)]
        public string ContactNameEN { get; set; }
        [StringLength(300)]
        public string ContactNumber { get; set; }
        [StringLength(300)]
        public string ContactTitle { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? BestAssessmentScore { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? AverageAssessmentScore { get; set; }
        public string CostAR { get; set; }
        public string CostEN { get; set; }
        public string LearningOutcomesAR { get; set; }
        public string LearningOutcomesEN { get; set; }
        public string SelectionCriteriaAR { get; set; }
        public string SelectionCriteriaEN { get; set; }
        public string ContactNameAR { get; set; }

        public string ReferenceNumber { get; set; }
        public string DescriptionAR { get; set; }
        public string DescriptionEN { get; set; }

        public int? MobImageID { get; set; }
        public int? ImageID { get; set; }
  

        [ForeignKey(nameof(AssessmentMatrixId))]
        [InverseProperty(nameof(AssessmentToolMatrix.Batches))]
        public virtual AssessmentToolMatrix AssessmentMatrix { get; set; }
        [ForeignKey(nameof(ProgrammeId))]
        [InverseProperty("Batches")]
        public virtual Programme Programme { get; set; }
        [ForeignKey(nameof(QuestionGroupId))]
        [InverseProperty("Batches")]
        public virtual QuestionGroup QuestionGroup { get; set; }
        [ForeignKey(nameof(VideoAssessmentId))]
        [InverseProperty("Batches")]
        public virtual VideoAssessment VideoAssessment { get; set; }
        [InverseProperty(nameof(Application.Batch))]
        public virtual ICollection<Application> Applications { get; set; }
        [InverseProperty(nameof(Assignment.Batch))]
        public virtual ICollection<Assignment> Assignments { get; set; }
        [InverseProperty(nameof(BatchAssessmentTool.Batch))]
        public virtual ICollection<BatchAssessmentTool> BatchAssessmentTools { get; set; }
        [InverseProperty(nameof(BatchInitiative.Batch))]
        public virtual ICollection<BatchInitiative> BatchInitiatives { get; set; }
        [InverseProperty(nameof(Event.Batch))]
        public virtual ICollection<Event> Events { get; set; }

        [InverseProperty(nameof(FolderBatch.Batch))]
        public virtual ICollection<FolderBatch> FolderBatches { get; set; }
        [InverseProperty(nameof(Glppermission.Batch))]
        public virtual ICollection<Glppermission> Glppermissions { get; set; }
        [InverseProperty(nameof(ProfileBatchAssessment.Batch))]
        public virtual ICollection<ProfileBatchAssessment> ProfileBatchAssessments { get; set; }
    }
}