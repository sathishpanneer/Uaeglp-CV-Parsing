﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("WorkflowActivity", Schema = "bbsf")]
    public partial class WorkflowActivity
    {
        public WorkflowActivity()
        {
            Participants = new HashSet<Participant>();
            WorkflowActivityOutcomeActivities = new HashSet<WorkflowActivityOutcome>();
            WorkflowActivityOutcomeNextActivities = new HashSet<WorkflowActivityOutcome>();
            WorkflowActivityVariables = new HashSet<WorkflowActivityVariable>();
            WorkflowErrors = new HashSet<WorkflowError>();
            WorkflowInstanceComments = new HashSet<WorkflowInstanceComment>();
            WorkflowInstanceTasks = new HashSet<WorkflowInstanceTask>();
            WorkflowInstances = new HashSet<WorkflowInstance>();
            WorkflowLogs = new HashSet<WorkflowLog>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string SysName { get; set; }
        [Required]
        [Column("NameEN")]
        [StringLength(255)]
        public string NameEn { get; set; }
        [Required]
        [Column("NameAR")]
        [StringLength(255)]
        public string NameAr { get; set; }
        [Column("WorkflowVersionID")]
        public int WorkflowVersionId { get; set; }
        [Column("ActivityTypeItemID")]
        public int ActivityTypeItemId { get; set; }
        [Column("TaskCorrelationTypeItemID")]
        public int? TaskCorrelationTypeItemId { get; set; }
        [Column("TemplateID")]
        public int? TemplateId { get; set; }
        public bool StopOnError { get; set; }
        [Column("TaskNameEN")]
        [StringLength(255)]
        public string TaskNameEn { get; set; }
        [Column("TaskNameAR")]
        [StringLength(255)]
        public string TaskNameAr { get; set; }
        [Column("TaskDescriptionEN")]
        [StringLength(255)]
        public string TaskDescriptionEn { get; set; }
        [Column("TaskDescriptionAR")]
        [StringLength(255)]
        public string TaskDescriptionAr { get; set; }
        [Column("TaskURL")]
        [StringLength(2083)]
        public string TaskUrl { get; set; }
        [Column("CanCompleteFromTME")]
        public bool CanCompleteFromTme { get; set; }
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

        [ForeignKey(nameof(ActivityTypeItemId))]
        [InverseProperty(nameof(LookupItem.WorkflowActivities))]
        public virtual LookupItem ActivityTypeItem { get; set; }
        [ForeignKey(nameof(TemplateId))]
        [InverseProperty("WorkflowActivities")]
        public virtual Template Template { get; set; }
        [ForeignKey(nameof(WorkflowVersionId))]
        [InverseProperty("WorkflowActivities")]
        public virtual WorkflowVersion WorkflowVersion { get; set; }
        [InverseProperty(nameof(Participant.WorkflowActivity))]
        public virtual ICollection<Participant> Participants { get; set; }
        [InverseProperty(nameof(WorkflowActivityOutcome.Activity))]
        public virtual ICollection<WorkflowActivityOutcome> WorkflowActivityOutcomeActivities { get; set; }
        [InverseProperty(nameof(WorkflowActivityOutcome.NextActivity))]
        public virtual ICollection<WorkflowActivityOutcome> WorkflowActivityOutcomeNextActivities { get; set; }
        [InverseProperty(nameof(WorkflowActivityVariable.Activity))]
        public virtual ICollection<WorkflowActivityVariable> WorkflowActivityVariables { get; set; }
        [InverseProperty(nameof(WorkflowError.Activity))]
        public virtual ICollection<WorkflowError> WorkflowErrors { get; set; }
        [InverseProperty(nameof(WorkflowInstanceComment.Activity))]
        public virtual ICollection<WorkflowInstanceComment> WorkflowInstanceComments { get; set; }
        [InverseProperty(nameof(WorkflowInstanceTask.Activity))]
        public virtual ICollection<WorkflowInstanceTask> WorkflowInstanceTasks { get; set; }
        [InverseProperty(nameof(WorkflowInstance.CurrentActivity))]
        public virtual ICollection<WorkflowInstance> WorkflowInstances { get; set; }
        [InverseProperty(nameof(WorkflowLog.Activity))]
        public virtual ICollection<WorkflowLog> WorkflowLogs { get; set; }
    }
}