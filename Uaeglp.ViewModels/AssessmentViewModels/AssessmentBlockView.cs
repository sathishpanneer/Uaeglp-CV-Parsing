using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentBlockView
	{
        public int ID { get; set; }

        public string IDEncrypted { get; set; }

        public string NameEn { get; set; }
        public string NameAr { get; set; }

        public int? LevelID { get; set; }

        public int AssessmentToolID { get; set; }

        public string AssessmentToolName { get; set; }

        public int SubassessmentToolID { get; set; }

        public string SubassessmentToolName { get; set; }

        public List<QuestionHeadView> Questionheads { get; set; }

        public int SubAssessmentTypeID { get; set; }
    }
}
