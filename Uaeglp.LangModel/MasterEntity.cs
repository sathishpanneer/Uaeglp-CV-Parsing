// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Uaeglp.LangModel
{
    public partial class MasterEntity
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string ShortcodeEn { get; set; }
        public string ShortcodeAr { get; set; }
        public string Logo { get; set; }
        public string Sequence { get; set; }
        public string CategoryEn { get; set; }
        public string CategoryAr { get; set; }
        public byte[] Img { get; set; }
        public string Filename { get; set; }
        public byte[] ImgAr { get; set; }
        public string FilenameAr { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int IsActive { get; set; }
    }
}