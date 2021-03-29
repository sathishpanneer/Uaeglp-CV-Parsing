using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class ReportConsolidateReportOverallView
    {
        public ReportConsolidateReportOverallView()
        {
            this.Profile = new ProfileReportView();
            this.AssessmentCategoryScores = new List<AssessmentCategoryScoreView>();
            this.WellBeingScalesScores = new List<DrillDownScoreView>();
            this.LeadershipPillarsScores = new List<DrillDownScoreView>();
            this.EQScalesScore = new List<DrillDownScoreView>();
            this.CareerInterestScalesScore = new List<DrillDownScoreView>();
            this.CognitiveSubBlocksScores = new List<DrillDownScoreView>();
            this.CompetenciesReport = new List<ProfileCompetencyScoreReportView>();
        }

        public ProfileReportView Profile { get; set; }

        public Decimal OverAllScore { get; set; }

        public List<AssessmentCategoryScoreView> AssessmentCategoryScores { get; set; }

        public List<DrillDownScoreView> WellBeingScalesScores { get; set; }

        public List<DrillDownScoreView> LeadershipPillarsScores { get; set; }

        public List<DrillDownScoreView> EQScalesScore { get; set; }

        public List<DrillDownScoreView> CareerInterestScalesScore { get; set; }

        public List<DrillDownScoreView> CognitiveSubBlocksScores { get; set; }

        public List<ProfileCompetencyScoreReportView> CompetenciesReport { get; set; }

        public string WellBeingNarrative { get; set; }

        public string EQNarrative { get; set; }

        public string CognitiveNarrative { get; set; }

        public string EnglishNarrative { get; set; }

        public string LeadershipNarrative { get; set; }

        public string LastAssessmentDate { get; set; }
    }

    public class DrillDownScoreView
    {
        public string Name { get; set; }

        public Decimal Score { get; set; }

        public int? TypeID { get; set; }
    }

    public class ProfileReportView
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public ProfileImgView ImageSrc { get; set; }

        public string About { get; set; }
    }

    public class ProfileImgView
    {
        public ProfileImgView()
        {
            this.ProgrammeswithTypeForPopOver = new List<ProgrammeCatgView>();
        }

        public string ImgeIDEncrypted { get; set; }

        public string DefaultImageSrc { get; set; }

        public int ProfileID { get; set; }

        public bool HideAlumni { get; set; }

        public string ExtraCalasses { get; set; }

        public List<ProgrammeCatgView> ProgrammeswithTypeForPopOver { get; set; }
    }

    public class ProgrammeCatgView
    {
        public ProgrammeCatgView()
        {
            this.Programmes = new List<ProgWithStatusView>();
        }

        public string Catg { get; set; }

        public List<ProgWithStatusView> Programmes { get; set; }
    }

    public class ProgWithStatusView
    {
        public string Name { get; set; }

        public string Status { get; set; }

        public int? StatusID { get; set; }
    }
}
