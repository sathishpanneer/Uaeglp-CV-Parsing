// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models.Reports
{
    [Table("ReportAssessmentCompetency", Schema = "report")]
    public partial class ReportAssessmentCompetency
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        [Column("AssessmentID")]
        public int AssessmentId { get; set; }
        [Column("AssessmentNameEN")]
        public string AssessmentNameEn { get; set; }
        [Column("AssessmentNameAR")]
        public string AssessmentNameAr { get; set; }
        [Column("CompetencyID")]
        public int CompetencyId { get; set; }
        [Column("CompetencyEN")]
        public string CompetencyEn { get; set; }
        [Column("CompetencyAR")]
        public string CompetencyAr { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BestScore { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal AverageScore { get; set; }
    }
}