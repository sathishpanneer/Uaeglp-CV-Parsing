using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class SubAssessmentToolView
    {
        public int Id { get; set; }

        public string NameEn { get; set; }
        
        public string NameAr { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }
        public int? SubAssessmentToolTypeId { get; set; }
        public int AssessmentToolId { get; set; }
        public decimal? Mean { get; set; }
        public decimal? StandardDeviation { get; set; }

        public LookupItemView SubAssessmentToolType { get; set; }
    }
}
