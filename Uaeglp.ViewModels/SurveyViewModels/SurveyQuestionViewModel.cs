using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.SurveyViewModels
{

    public class SurveyQuestionViewModel
    {

        public int SurveyId { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public bool IsPublished { get; set; }
        public bool IsSurveyAlreadySubmitted { get; set; }
        public IList<SurveyQuestionView> SurveyQuestions { get; set; } = new List<SurveyQuestionView>();
    }
}
