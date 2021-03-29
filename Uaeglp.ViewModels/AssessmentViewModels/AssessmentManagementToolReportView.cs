using System.Collections.Generic;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class AssessmentManagementToolReportView
	{
        public AssessmentManagementToolReportView()
        {
            this.profileCompetencyScoreReports = new List<ProfileCompetencyScoreReportView>();
            this.ManagementCognitive = new ManagementCognitiveView();
        }

        public int AssessmentToolID { get; set; }

        public int AssessmentToolCategoryID { get; set; }

        public EnglishArabicView AssessmentToolName { get; set; }

        public string CompletedOn { get; set; }

        public EnglishArabicView ProfileName { get; set; }

        public List<ProfileCompetencyScoreReportView> profileCompetencyScoreReports { get; set; }

        public int OverallScore { get; set; }

        public int AssessmnetToolCategoryID { get; internal set; }

        public EnglishArabicView OverAllResultDescription { get; set; }

        public ManagementCognitiveView ManagementCognitive { get; set; }

        public string ViewPath { get; set; }

        public string FileName { get; set; }
    }
}
