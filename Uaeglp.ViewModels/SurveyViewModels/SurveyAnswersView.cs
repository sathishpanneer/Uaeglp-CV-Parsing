using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.ViewModels.SurveyViewModels
{
    public class SurveyAnswersView
    {
        public int ProfileId { get; set; }
        public int SurveyId { get; set; }
        public List<SurveyAnswerViewModel> AnswerList { get; set; } = new List<SurveyAnswerViewModel>();
    }

    public class SurveyAnswerViewModel
    {
        public int QuestionId { get; set; }
        public SurveyQuestionType QuestionTypeItemId { get; set; }
        public string Answer { get; set; }
        public List<int> MultiSelectionIds { get; set; } = new List<int>();
        public int SingleSelectionId { get; set; }
        public IFormFile FileData { get; set; }
    }
}
