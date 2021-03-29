using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.KnowledgeHub
{
    public class KnowledgeHubCourseView
    {
        public int ID { get; set; }
        public string IDEncyrpted { get; set; }
        public string CourseProviderID { get; set; }
        public string NameEN { get; set; }
        public string NameAR { get; set; }
        public string DescriptionEN { get; set; }
        public string DescriptionAR { get; set; }
        public string AboutEN { get; set; }
        public string AboutAR { get; set; }
        public int? CategoryID { get; set; }
        public string CategoryNameEn { get; set; }
        public string CategoryNameAr { get; set; }
        public string IframeUrlEN { get; set; }
        public string IframeUrlAR { get; set; }
        public string Language { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public DateTime? ProviderLastModifiedDate { get; set; }
        public string ImageURL { get; set; }
        public string CoverImageURL { get; set; }
        public bool ShowProviderIcon { get; set; }
        public bool IsHide { get; set; }
        public int CourseTypeID { get; set; }
        public int ProviderTypeID { get; set; }
        public string CourseTypeEn { get; set; }
        public string CourseTypeAr { get; set; }
        public string ProviderTypeEn { get; set; }
        public string ProviderTypeAr { get; set; }
        public bool IsFavourite { get; set; }
    }
}
