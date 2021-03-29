using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class QuestionItemView
	{
        public int ID { get; set; }

        public string IDEncrypted { get; set; }

        public EnglishArabicView Name { get; set; }

        public QuestionItemDirection QuestionDirection { get; set; }

        public int? SubScaleID { get; set; }

        public string SubScaleName { get; set; }

        public int? QuestionheadID { get; set; }

        public string QuestioheadName { get; set; }

        public int? AssessmentToolID { get; set; }

        public string AssessmentToolName { get; set; }

        public bool HasSubScale { get; set; }

        public int? ScaleID { get; set; }

        public string ScaleName { get; set; }

        public int? CompetencyID { get; set; }

        public string CompetencyName { get; set; }

        public int? LevelID { get; set; }

        public int? TypeID { get; set; }

        public bool HasQuestionDirect { get; set; }

        public int? AssessmentTypeID { get; set; }

        public int? SubAssessmentTypeID { get; set; }

        public List<QAnswerView> Answers { get; set; }

        public bool HasQuestionHead { get; set; }

        public int? ImageID { get; set; }
        public int? MobImageID { get; set; }

        public FileViewModel Image { get; set; }

        public int? TimeTaken { get; set; }
    }
}
