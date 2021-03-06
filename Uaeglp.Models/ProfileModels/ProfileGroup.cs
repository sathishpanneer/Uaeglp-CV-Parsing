// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("Profile_Groups")]
    public partial class ProfileGroup
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("GroupID")]
        public int GroupId { get; set; }
        [Column("ProfileID")]
        public int ProfileId { get; set; }
        public bool? IsFollowed { get; set; }

        [ForeignKey(nameof(GroupId))]
        [InverseProperty("ProfileGroups")]
        public virtual Group Group { get; set; }
        [ForeignKey(nameof(ProfileId))]
        [InverseProperty("ProfileGroups")]
        public virtual Profile Profile { get; set; }
    }
}