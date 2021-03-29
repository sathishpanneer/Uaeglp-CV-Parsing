using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentHeadAnswersView
	{
		public List<QuestionScoreView> QuestionItemScores { get; set; }
		public List<AssessmentNavigationObjectView> AssessmentNavigationObject { get; set; }
	}
}
