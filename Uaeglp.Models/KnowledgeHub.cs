// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("KnowledgeHub")]
    public partial class KnowledgeHub
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string ArticleTitleEn { get; set; }
        [Required]
        [StringLength(100)]
        public string ArticleTitleAr { get; set; }
        [Required]
        [StringLength(100)]
        public string AuthorTitleEn { get; set; }
        [Required]
        [StringLength(100)]
        public string AuthorTitleAr { get; set; }
        [Column("ImageID")]
        public Guid? ImageId { get; set; }
        [Column("CategoryID")]
        public int CategoryId { get; set; }
        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; }
        [Required]
        [StringLength(256)]
        public string ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Modified { get; set; }
        [Required]
        [Column("HTMLContentEN")]
        public string HtmlcontentEn { get; set; }
        [Required]
        [Column("HTMLContentAR")]
        public string HtmlcontentAr { get; set; }

        [ForeignKey(nameof(CategoryId))]
        [InverseProperty(nameof(LookupItem.KnowledgeHubs))]
        public virtual LookupItem Category { get; set; }
    }
}