using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class QuestionWithAnswerView
	{
        public int SurveyID { get; set; }

        public int ProfileAssessmentToolID { get; set; }

        public string SurveyIDEncrypted { get; set; }

        public int ProfielID { get; set; }

        public string ProfielIDEncrypted { get; set; }

        public bool Participated { get; set; }

        public List<QuestionView> Questions { get; set; }

        public List<QuestionAnswerView> QuestionsAnswer { get; set; }

        public QuestionWithAnswerView()
        {
            this.Questions = new List<QuestionView>();
            this.QuestionsAnswer = new List<QuestionAnswerView>();
        }
    }
}
