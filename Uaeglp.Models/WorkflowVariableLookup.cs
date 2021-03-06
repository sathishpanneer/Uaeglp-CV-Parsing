// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("WorkflowVariableLookup", Schema = "bbsf")]
    public partial class WorkflowVariableLookup
    {
        public WorkflowVariableLookup()
        {
            WorkflowInstanceVariableVariableLookups = new HashSet<WorkflowInstanceVariableVariableLookup>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(2000)]
        public string Value { get; set; }
        [Required]
        [Column("NameEN")]
        [StringLength(255)]
        public string NameEn { get; set; }
        [Required]
        [Column("NameAR")]
        [StringLength(255)]
        public string NameAr { get; set; }
        [Column("VariableID")]
        public int VariableId { get; set; }
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

        [ForeignKey(nameof(VariableId))]
        [InverseProperty(nameof(WorkflowVariable.WorkflowVariableLookups))]
        public virtual WorkflowVariable Variable { get; set; }
        [InverseProperty(nameof(WorkflowInstanceVariableVariableLookup.InstanceVariable))]
        public virtual ICollection<WorkflowInstanceVariableVariableLookup> WorkflowInstanceVariableVariableLookups { get; set; }
    }
}