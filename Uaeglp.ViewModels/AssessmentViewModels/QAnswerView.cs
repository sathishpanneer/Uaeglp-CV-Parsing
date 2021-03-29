using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class QAnswerView
	{
        public int ID { get; set; }

        public string IDEncrypted { get; set; }

        public EnglishArabicView Name { get; set; }

        public EnglishArabicView Description { get; set; }

        public bool IsCorrectAnswer { get; set; }

        public int QuestionID { get; set; }

        public string QuestionName { get; set; }

        public int? Score { get; set; }

        public int? AssessmenttoolTypeId { get; set; }

        public int? ImageID { get; set; }
        public int? MobImageID { get; set; }

        public FileViewModel Image { get; set; }

        public int? SubAssessmenttoolTypeId { get; set; }
    }
}
