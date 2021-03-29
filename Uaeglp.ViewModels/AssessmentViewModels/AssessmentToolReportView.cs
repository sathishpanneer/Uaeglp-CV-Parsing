using System;
using System.Collections.Generic;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentToolReportView
	{
        public int ProfileID { get; set; }

        public Decimal Score { get; set; }

        public int AssessmentToolID { get; set; }

        public int Order { get; set; }
        public int Trial { get; set; }


        public string ProfileNameEn { get; set; }
        public string ProfileNameAr { get; set; }

        public string ProfileEmail { get; set; }

        public string ProfileMobile { get; set; }

        public bool IsCompleted { get; set; }

        public bool? IsFailed { get; set; }

        public int ProfileAssessmentID { get; set; }
        public bool FeedbackSubmited { get; set; }

        public string ProfileAssessmentIDEncrypted { get; set; }

        public bool InProgress { get; set; }
		public string AssessmentToolName { get; set; }
		public string CompletedDate { get; set; }
		public bool IsExisted { get; set; }
		public bool IsCareer { get; set; }
		public bool IsPersonality { get; set; }
		public int AssessmneToolCategoryID { get; set; }
        public object Groups { get; set; }
        public EnglishArabicView AssessmentName { get; set; }
    }
}
