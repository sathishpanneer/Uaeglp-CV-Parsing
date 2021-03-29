using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class QuestionWithAnswerView_New
    {
        public int ProfileAssessmentToolID { get; set; }
        public List<QuestionAnswerView_New> QuestionsAnswer { get; set; }

        public QuestionWithAnswerView_New()
        {
             this.QuestionsAnswer = new List<QuestionAnswerView_New>();
        }
    }
    public class QuestionAnswerView_New
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public int? SelectedOptionId { get; set; }
        public bool? YnquestionAnswer { get; set; }
        public int? Scale { get; set; }
        public int QuestionTypeItemID { get; set; }
    }
}
