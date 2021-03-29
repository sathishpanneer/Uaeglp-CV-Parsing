﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("Industry")]
    public partial class Industry
    {
        public Industry()
        {
            ProfileWorkExperiences = new HashSet<ProfileWorkExperience>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
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
        [Required]
        [Column("NameEN")]
        [StringLength(300)]
        public string NameEn { get; set; }
        [Required]
        [Column("NameAR")]
        [StringLength(300)]
        public string NameAr { get; set; }

        [InverseProperty(nameof(ProfileWorkExperience.Industry))]
        public virtual ICollection<ProfileWorkExperience> ProfileWorkExperiences { get; set; }
    }
}