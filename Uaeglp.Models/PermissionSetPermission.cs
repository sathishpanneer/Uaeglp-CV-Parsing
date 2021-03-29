﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("PermissionSet_Permission", Schema = "bbsf")]
    public partial class PermissionSetPermission
    {
        [Key]
        [Column("PermissionSetID")]
        public int PermissionSetId { get; set; }
        [Key]
        [Column("PermissionID")]
        public int PermissionId { get; set; }

        [ForeignKey(nameof(PermissionId))]
        [InverseProperty("PermissionSetPermissions")]
        public virtual Permission Permission { get; set; }
        [ForeignKey(nameof(PermissionSetId))]
        [InverseProperty("PermissionSetPermissions")]
        public virtual PermissionSet PermissionSet { get; set; }
    }
}