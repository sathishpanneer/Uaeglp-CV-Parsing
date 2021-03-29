using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class ScaleView
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string LowDescriptionEn { get; set; }
        public string LowDescriptionAr { get; set; }
        public string HighDescriptionEn { get; set; }
        public string HighDescriptionAr { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public int FactorId { get; set; }
        public decimal? Mean { get; set; }
        public decimal? StandardDeviation { get; set; }
        public string NarrativeLowDescriptionEn { get; set; }
        public string NarrativeLowDescriptionAr { get; set; }
        public string NarrativeBelowAverageDescriptionEn { get; set; }
        public string NarrativeBelowAverageDescriptionAr { get; set; }
        public string NarrativeAverageDescriptionEn { get; set; }
        public string NarrativeAverageDescriptionAr { get; set; }
        public string NarrativeAboveAverageDescriptionEn { get; set; }
        public string NarrativeAboveAverageDescriptionAr { get; set; }
        public string NarrativeHighDescriptionEn { get; set; }
        public string NarrativeHighDescriptionAr { get; set; }
        public string LowestPreferedJobTypesDescriptionEn { get; set; }
        public string LowestPreferedJobTypesDescriptionAr { get; set; }
        public string HighestPreferedJobTypesDescriptionEn { get; set; }
        public string HighestPreferedJobTypesDescriptionAr { get; set; }
    }
}
