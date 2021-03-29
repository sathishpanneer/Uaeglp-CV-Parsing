﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("AssessmentToolMatrix")]
    public partial class AssessmentToolMatrix
    {
        public AssessmentToolMatrix()
        {
            AssessmentGroups = new HashSet<AssessmentGroup>();
            AssignedAssessments = new HashSet<AssignedAssessment>();
            Batches = new HashSet<Batch>();
            MatricesTool = new HashSet<MatrixTool>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [Column("TitleEN")]
        [StringLength(255)]
        public string TitleEn { get; set; }
        [Required]
        [Column("TitleAR")]
        [StringLength(255)]
        public string TitleAr { get; set; }
        [Required]
        [Column("DescriptionEN")]
        [StringLength(255)]
        public string DescriptionEn { get; set; }
        [Required]
        [Column("DescriptionAR")]
        [StringLength(255)]
        public string DescriptionAr { get; set; }
        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; }
        [Required]
        [StringLength(256)]
        public string ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        public bool IsOverall { get; set; }

        [InverseProperty(nameof(AssessmentGroup.AssessmentToolMatrix))]
        public virtual ICollection<AssessmentGroup> AssessmentGroups { get; set; }
        [InverseProperty(nameof(AssignedAssessment.AssessmentToolMatrix))]
        public virtual ICollection<AssignedAssessment> AssignedAssessments { get; set; }
        [InverseProperty(nameof(Batch.AssessmentMatrix))]
        public virtual ICollection<Batch> Batches { get; set; }
        [InverseProperty(nameof(MatrixTool.Matrix))]
        public virtual ICollection<MatrixTool> MatricesTool { get; set; }
    }
}