﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uaeglp.Models
{
    [Table("EmailHeaderAndFooterTemplate", Schema = "bbsf")]
    public partial class EmailHeaderAndFooterTemplate
    {
        public EmailHeaderAndFooterTemplate()
        {
            TemplateInfos = new HashSet<TemplateInfo>();
        }

        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [Column("NameEN")]
        [StringLength(100)]
        public string NameEn { get; set; }
        [Required]
        [Column("NameAR")]
        [StringLength(100)]
        public string NameAr { get; set; }
        [Column("HeaderEN")]
        public string HeaderEn { get; set; }
        [Column("HeaderAR")]
        public string HeaderAr { get; set; }
        [Column("FooterEN")]
        public string FooterEn { get; set; }
        [Column("FooterAR")]
        public string FooterAr { get; set; }
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
        public byte[] RowVersion { get; set; }
        [Column("HeadSectionEN")]
        public string HeadSectionEn { get; set; }
        [Column("HeadSectionAR")]
        public string HeadSectionAr { get; set; }
        [Column("HeadTagAttributesEN")]
        public string HeadTagAttributesEn { get; set; }
        [Column("HeadTagAttributesAR")]
        public string HeadTagAttributesAr { get; set; }
        public bool IsMoveCssInline { get; set; }

        [InverseProperty(nameof(TemplateInfo.EmailHeaderAndFooterTemplate))]
        public virtual ICollection<TemplateInfo> TemplateInfos { get; set; }
    }
}