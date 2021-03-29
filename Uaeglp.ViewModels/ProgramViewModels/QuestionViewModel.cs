using System;
using System.Collections.Generic;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels.ProgramViewModels
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public string TextEn { get; set; }
        public string TextAr { get; set; }
        public ApplicationQuestionType QuestionTypeItemId { get; set; }
        public int WordCount { get; set; }
        public bool IsPredefined { get; set; }
        public LookupItemView QuestionTypeItem { get; set; }
        public List<OptionViewModel> Options { get; set; } = new List<OptionViewModel>();

        public QuestionAnswerViewModel Answered { get; set; }

    }
}
