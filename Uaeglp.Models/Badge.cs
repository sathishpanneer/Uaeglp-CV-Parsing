// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("Badge")]
    public partial class Badge
    {
        public Badge()
        {
            BadgeRequests = new HashSet<BadgeRequest>();
            Profiles = new HashSet<Profile>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [Column("TitleEN")]
        public string TitleEn { get; set; }
        [Required]
        [Column("TitleAR")]
        public string TitleAr { get; set; }
        [Column("DescriptionEN")]
        public string DescriptionEn { get; set; }
        [Column("DescriptionAR")]
        public string DescriptionAr { get; set; }
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
        [Column("IconID")]
        public int IconId { get; set; }
        [Column("BackgroundImageID")]
        public int BackgroundImageId { get; set; }
        public string Color { get; set; }
        [Column("SummaryEN")]
        public string SummaryEn { get; set; }
        [Column("SummaryAR")]
        public string SummaryAr { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsUpdated { get; set; }
        public int MinimumPoints { get; set; }

        [InverseProperty(nameof(BadgeRequest.Badge))]
        public virtual ICollection<BadgeRequest> BadgeRequests { get; set; }
        [InverseProperty(nameof(Profile.Badge))]
        public virtual ICollection<Profile> Profiles { get; set; }
    }
}