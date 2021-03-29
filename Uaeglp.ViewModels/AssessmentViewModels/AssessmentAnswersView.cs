using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentAnswersView
	{
		public List<QuestionScoreView> QuestionItemScores { get; set; }

		public bool HasSubScale { get; set; }
		public bool HasQuestionDirect { get; set; }
		public string QuestionIDs { get; set; }
	}
}
