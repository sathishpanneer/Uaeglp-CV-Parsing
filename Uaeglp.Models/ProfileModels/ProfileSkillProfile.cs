// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("ProfileSkillProfile")]
    public partial class ProfileSkillProfile
    {

        [Key, Column("ProfileSkill_ID", Order = 0)]
        public int Id { get; set; }

        [Key, Column("Profile_ID")]
        public int ProfileId { get; set; }

        [ForeignKey(nameof(ProfileId))]
        public virtual Profile Profile { get; set; }

        [ForeignKey(nameof(Id))]
        public virtual ProfileSkill ProfileSkill { get; set; }
    }
}