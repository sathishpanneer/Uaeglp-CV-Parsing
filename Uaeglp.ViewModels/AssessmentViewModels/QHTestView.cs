using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
    public class QHTestView
    {
        public int ID { get; set; }

        public string IDEncrypted { get; set; }

        public EnglishArabicView Name { get; set; }

        public FileViewModel QHImage { get; set; }
        public int? QHImageId { get; set; }

        public string SubAssessmentToolName { get; set; }

        public string BlockName { get; set; }

        public QuestionItemView Questions { get; set; }

        public int SubAssessementToolTypeID { get; set; }
    }
}
