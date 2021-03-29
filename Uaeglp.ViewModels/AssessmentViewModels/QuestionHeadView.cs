using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class QuestionHeadView
	{
        public int ID { get; set; }
        public string IDEncrypted { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public List<QuestionItemView> Questions { get; set; }
        public int AssessmentToolID { get; set; }
        public string AssessmentToolIDEncrypted { get; set; }
        public string AssessmentToolName { get; set; }
        public int AssessmentToolIDType { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public int AssessmentBlockID { get; set; }
        public string AssessmentBlockName { get; set; }
        public int SubassessmentToolIDType { get; set; }
        public int SubassessmentToolID { get; set; }
        public string SubassessmentToolName { get; set; }
        public int ImageID { get; set; }
    }
}
