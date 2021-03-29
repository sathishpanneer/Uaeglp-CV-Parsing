using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.AssessmentViewModels;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class QuickHeadTestView
	{
        public int ID { get; set; }

        public string IDEncrypted { get; set; }

        public EnglishArabicView Name { get; set; }

        public FileViewModel QHImage { get; set; }
        public int? QHImageId { get; set; }
        public int? QHMobImageId { get; set; }

        public string SubAssessmentToolName { get; set; }

        public string BlockName { get; set; }

        public QuestionItemView Questions { get; set; }

        public int SubAssessementToolTypeID { get; set; }
    }
}
