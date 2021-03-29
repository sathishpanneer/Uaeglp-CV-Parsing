using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class ProfileCompetencyScoreReportView
    {
        public EnglishArabicView CompetencyName { get; set; }

        public EnglishArabicView CompetencyDescription { get; set; }

        public EnglishArabicView CompetencyResultDescription { get; set; }

        public EnglishArabicView CompetencyResultNarrativeDescription { get; set; }

        public Decimal Score { get; set; }

        public int CompetencyID { get; set; }
    }
}
