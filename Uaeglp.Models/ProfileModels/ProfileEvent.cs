﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("Profile_Events")]
    public partial class ProfileEvent
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("EventID")]
        public int EventId { get; set; }
        [Column("ProfileID")]
        public int ProfileId { get; set; }
        [Column("EventStatusItemID")]
        public int EventStatusItemId { get; set; }

        [ForeignKey(nameof(EventId))]
        [InverseProperty("ProfileEvents")]
        public virtual Event Event { get; set; }
        [ForeignKey(nameof(EventStatusItemId))]
        [InverseProperty(nameof(LookupItem.ProfileEvents))]
        public virtual LookupItem EventStatusItem { get; set; }
        [ForeignKey(nameof(ProfileId))]
        [InverseProperty("ProfileEvents")]
        public virtual Profile Profile { get; set; }
    }
}