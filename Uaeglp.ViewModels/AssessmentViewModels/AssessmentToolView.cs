using System;
using System.Collections.Generic;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class AssessmentToolView
    {
        public AssessmentToolView()
        {
            this.Factors = new List<FactorView>();
            this.SubAssessmenttools = new List<SubAssessmentToolView>();
        }

        public int ID { get; set; }

        public EnglishArabicView CoverPage { get; set; }

        public string IDEncrypted { get; set; }

        public int ValidityRangeNumber { get; set; }

        public EnglishArabicView Name { get; set; }

        public int? AssessmentToolTypeID { get; set; }

        public int AssessmentToolCategID { get; set; }

        public string AssessmentToolType { get; set; }

        public List<FactorView> Factors { get; set; }

        public bool Valid { get; set; }

        public bool HasSubScale { get; set; }

        public bool HasQuestionDirect { get; set; }

        public bool HasQuestionHead { get; set; }

        public int MatrixID { get; set; }
        public string DurationEn { get; set; }
        public string DurationAr { get; set; }
        public string InstructionsEn { get; set; }
        public string InstructionsAr { get; set; }
        public Decimal? Mean { get; set; }

        public Decimal? StandardDeviation { get; set; }

        public EnglishArabicView Description { get; set; }

        public string CoverPageName { get; set; }

        public Guid ImageID { get; set; }
        public Guid? MobImageID { get; set; }

        public FileViewModel Image { get; set; }

        public TimeDiffereceView TimeLeft { get; set; }

        public List<SubAssessmentToolView> SubAssessmenttools { get; set; }

        public bool IsRequiredAssessment { get; set; }

        public DateTime AssessmentEndDate { get; set; }

        public DateTime AssessmentStartDate { get; set; }

        public bool HasCompetency { get; set; }

        public bool IsDefaultVisible { get; set; }

        public bool IsProcessing { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsFailed { get; set; }

        public bool IsAssignedAssessment { get; set; }

        public object Groups { get; set; }

        public int TotalQuestions { get; set; }
    }
}
