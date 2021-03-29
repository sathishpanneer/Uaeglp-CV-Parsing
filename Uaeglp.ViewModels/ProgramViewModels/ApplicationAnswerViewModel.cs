using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.ViewModels.ProgramViewModels
{
    public class ApplicationAnswerViewModel
    {
        public int QuestionId { get; set; }

        public ApplicationQuestionType QuestionType { get; set; }

        public string Text { get; set; }

        public bool? YesNoAnswer { get; set; }

        public int? Scale { get; set; }

        public IFormFile AnswerFile { get; set; }

        public int? SelectedOptionId { get; set; }

        public string MultipleChoice { get; set; }

    }
}
