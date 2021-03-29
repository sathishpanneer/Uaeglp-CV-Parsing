﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("Profile_Tags")]
    public partial class ProfileTag
    {
        [Key]
        [Column("ProfileID")]
        public int ProfileId { get; set; }
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [ForeignKey(nameof(Id))]
        [InverseProperty(nameof(LookupItem.ProfileTags))]
        public virtual LookupItem IdNavigation { get; set; }
        [ForeignKey(nameof(ProfileId))]
        [InverseProperty("ProfileTags")]
        public virtual Profile Profile { get; set; }
    }
}