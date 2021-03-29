using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.SurveyViewModels
{
    public class SurveyQuestionFieldView
    {
        public int Id { get; set; }
        public int SurveyQuestionId { get; set; }
        public string TextEn { get; set; }
        public string TextAr { get; set; }
        public int DisplayOrder { get; set; }
        public int? LinkedSurveyQuestionId { get; set; }
    }
}
