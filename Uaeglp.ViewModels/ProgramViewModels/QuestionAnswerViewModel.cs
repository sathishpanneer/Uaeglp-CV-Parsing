using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels.ProgramViewModels
{
    public class QuestionAnswerViewModel
    {
        public int Id { get; set; }
        public int? ApplicationId { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public int? ProfileId { get; set; }
        public int? SelectedOptionId { get; set; }
        public bool? YnquestionAnswer { get; set; }
        public int? Scale { get; set; }
        public int? AnswerFileId { get; set; }
        public int? InitiativeId { get; set; }
        public int? SurveyProfileInfoId { get; set; }
        public int? AssessmentReportFeedbackId { get; set; }
        public virtual FileView AnswerFile { get; set; }
        public List<QuestionAnswerOptionViewModel> MultiSelectedOptionId { get; set; }
    }
    public class QuestionAnswerOptionViewModel
    {

        public int QuestionanswerID { get; set; }
        public int optionID { get; set; }
    }
}
