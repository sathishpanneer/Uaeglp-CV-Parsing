using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentQuestionItemReportView
	{
		public List<QuestionItemScoreView> Questions { get; set; }

		public string AssessmentName { get; set; }

		public int AssesmentID { get; set; }

		public string ProfileName { get; set; }

		public string ProfileEmail { get; set; }
	}
}
