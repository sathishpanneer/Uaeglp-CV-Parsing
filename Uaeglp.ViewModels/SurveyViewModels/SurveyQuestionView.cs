using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.SurveyViewModels
{
    public class SurveyQuestionView
    {
        public int Id { get; set; }
        public int SurveyID { get; set; }
        public int QuestionTypeItemId { get; set; }
        public string QuestionTextEn { get; set; }
        public string QuestionTextAr { get; set; }
        public string PlaceHolderEn { get; set; }
        public string PlaceHolderAr { get; set; }
        public string AcceptedFiles { get; set; }
        public int MaxLength { get; set; }
        public int MaxFileSize { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsRequired { get; set; }
        public bool IsVisible { get; set; }
        public IList<SurveyQuestionFieldView> SurveyQuestionFields { get; set; } = new List<SurveyQuestionFieldView>();
    }
}
