using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentDetailsView
	{
        public int AppId { get; set; }

        public Decimal? AssessmentTotalScore { get; set; }

        public List<QuestionView> Questions { get; set; }

        public List<QuestionAnswerView> QuestionsAnswer { get; set; }

        public AssessmentDetailsView()
        {
            this.Questions = new List<QuestionView>();
            this.QuestionsAnswer = new List<QuestionAnswerView>();
        }
    }
}
