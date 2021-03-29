﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("GovernmentEntity_coordinators")]
    public partial class GovernmentEntityCoordinator
    {
        [Key]
        [Column("ClientUser_ID")]
        public int ClientUserId { get; set; }
        [Key]
        [Column("GovernmentEntity_ID")]
        public int GovernmentEntityId { get; set; }

        [ForeignKey(nameof(ClientUserId))]
        [InverseProperty(nameof(User.GovernmentEntityCoordinators))]
        public virtual User ClientUser { get; set; }
        [ForeignKey(nameof(GovernmentEntityId))]
        [InverseProperty("GovernmentEntityCoordinators")]
        public virtual GovernmentEntity GovernmentEntity { get; set; }
    }
}