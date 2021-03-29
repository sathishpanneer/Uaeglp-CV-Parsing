using System;
using System.Collections.Generic;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class QuestionItemScoreView
	{
		public int ID { get; set; }

		public int ProfileID { get; set; }

		public EnglishArabicView Question { get; set; }

		public string QuestionHead { get; set; }

		public int Score { get; set; }

		public Decimal ViewScore { get; set; }

		public string Answer { get; set; }

		public int? AnswerID { get; set; }

		public int? TimeTaken { get; set; }

		public QuestionItemDirection QuestionDirection { get; set; }

		public int Order { get; set; }

		public string CompletedDate { get; set; }

		public string ProfileName { get; set; }

		public string ProfileEmail { get; set; }

		public List<QAnswerView> Answers { get; set; }

		public string SubAssessmenToolName { get; set; }

		public int? SubAssesmentToolTypeID { get; set; }

		public string BlockName { get; set; }
	}
}
