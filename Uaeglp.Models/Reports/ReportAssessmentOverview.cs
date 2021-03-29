﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models.Reports
{
    [Table("ReportAssessmentOverview", Schema = "report")]
    public partial class ReportAssessmentOverview
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
        [Column("BatchID")]
        public int? BatchId { get; set; }
        [Column("BatchNameEN")]
        public string BatchNameEn { get; set; }
        [Column("BatchNameAR")]
        public string BatchNameAr { get; set; }
        public bool IsOverView { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BestScore { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal AverageScore { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? EnglishScoreBestPercentage { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? EnglishScoreAveragePercentage { get; set; }
        public bool IsOverAll { get; set; }
        [Column("TabItemID")]
        public int TabItemId { get; set; }
    }
}