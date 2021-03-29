using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class SubScaleView
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
        public int ScaleId { get; set; }
        public decimal? Mean { get; set; }
        public decimal? StandardDeviation { get; set; }
    }
}
