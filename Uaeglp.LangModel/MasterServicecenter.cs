// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Uaeglp.LangModel
{
    public partial class MasterServicecenter
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? EntityId { get; set; }
        public int? CityId { get; set; }
        public string AddressEn { get; set; }
        public string AddressAr { get; set; }
        public int? AreaId { get; set; }
        public int? Sequence { get; set; }
        public string Logo { get; set; }
        public byte[] Img { get; set; }
        public string Filename { get; set; }
        public byte[] ImgAr { get; set; }
        public string FilenameAr { get; set; }
        public TimeSpan? Opentime { get; set; }
        public TimeSpan? Closetime { get; set; }
        public int? FromDay { get; set; }
        public int? ToDay { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int IsActive { get; set; }
        public string Infratype { get; set; }
        public string KhCentercode { get; set; }
    }
}