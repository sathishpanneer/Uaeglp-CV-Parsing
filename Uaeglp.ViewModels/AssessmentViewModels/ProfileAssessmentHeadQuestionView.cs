using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class ProfileAssessmentHeadQuestionView
	{
        public QuickHeadTestView QuestionsHead { get; set; }

        public int AssessmentToolID { get; set; }

        public string AssessmentToolName { get; set; }

        public int ProfileID { get; set; }

        public int TotalTestCount { get; set; }

        public bool IsLastCount { get; set; }

        public int TotalAnsweredCount { get; set; }

        public int AssessmentToolType { get; set; }

        public List<AssessmentNavigationObjectView> _assessmentNavigationObject { get; set; }

        public int Order { get; set; }

        public bool IsFailed { get; set; }
    }
}
