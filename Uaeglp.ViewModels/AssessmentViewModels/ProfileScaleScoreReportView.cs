using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class ProfileScaleScoreReportView
    {
        public EnglishArabicView ScaleName { get; set; }

        public EnglishArabicView ScaleLowDescription { get; set; }

        public EnglishArabicView ScaleHighDescription { get; set; }

        public int Score { get; set; }

        public int ScaleID { get; set; }
    }
}
