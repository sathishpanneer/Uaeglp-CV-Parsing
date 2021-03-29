﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("Criterion")]
    public partial class Criterion1
    {
        public Criterion1()
        {
            ProfileVideoAssessmentCriteriaScores = new HashSet<ProfileVideoAssessmentCriteriaScore>();
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
        [Column(TypeName = "decimal(20, 10)")]
        public decimal? Weight { get; set; }
        [Column("CriteriaID")]
        public int CriteriaId { get; set; }

        [ForeignKey(nameof(CriteriaId))]
        [InverseProperty(nameof(VidAssesCriterion.Criterion1s))]
        public virtual VidAssesCriterion Criteria { get; set; }
        [InverseProperty(nameof(ProfileVideoAssessmentCriteriaScore.Criterion))]
        public virtual ICollection<ProfileVideoAssessmentCriteriaScore> ProfileVideoAssessmentCriteriaScores { get; set; }
    }
}