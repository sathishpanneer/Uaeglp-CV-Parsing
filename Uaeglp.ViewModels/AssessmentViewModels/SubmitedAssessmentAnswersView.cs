using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class SubmitedAssessmentAnswersView
	{
		public ProfileAssessmentQuestionView ProfileAssessmentQuestionView { get; set; }
		public int SkippedCount { get; set; }
		public int TotalTestCount { get; set; }
	}
}
