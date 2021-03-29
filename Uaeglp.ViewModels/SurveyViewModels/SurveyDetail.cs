using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.SurveyViewModels
{
    public class SurveyDetail
    {
        public int SurveyId { get; set; }
        public string TitleEn { get; set; } 
        public string TitleAr { get; set; }
        public string DescriptionEn { get; set; }        
        public string DescriptionAr { get; set; }
        public ICollection<SurveyQuestionView> SurveyQuestions { get; set; }
    }
}
