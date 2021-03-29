﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("WorkflowActivityOutcome", Schema = "bbsf")]
    public partial class WorkflowActivityOutcome
    {
        public WorkflowActivityOutcome()
        {
            Tmetasks = new HashSet<Tmetask>();
            WorkflowLogs = new HashSet<WorkflowLog>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [Column("NameEN")]
        [StringLength(255)]
        public string NameEn { get; set; }
        [Required]
        [Column("NameAR")]
        [StringLength(255)]
        public string NameAr { get; set; }
        [Required]
        [StringLength(100)]
        public string SysName { get; set; }
        [Column("ActivityID")]
        public int ActivityId { get; set; }
        [Column("NextActivityID")]
        public int? NextActivityId { get; set; }
        public bool CanAddComment { get; set; }
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

        [ForeignKey(nameof(ActivityId))]
        [InverseProperty(nameof(WorkflowActivity.WorkflowActivityOutcomeActivities))]
        public virtual WorkflowActivity Activity { get; set; }
        [ForeignKey(nameof(NextActivityId))]
        [InverseProperty(nameof(WorkflowActivity.WorkflowActivityOutcomeNextActivities))]
        public virtual WorkflowActivity NextActivity { get; set; }
        [InverseProperty(nameof(Tmetask.TaskOutcomeItem))]
        public virtual ICollection<Tmetask> Tmetasks { get; set; }
        [InverseProperty(nameof(WorkflowLog.ActivityOutcome))]
        public virtual ICollection<WorkflowLog> WorkflowLogs { get; set; }
    }
}