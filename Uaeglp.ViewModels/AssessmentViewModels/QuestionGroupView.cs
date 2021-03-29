using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class QuestionGroupView
	{
        public int QuestionsCount;

        public int ID { get; set; }

        public string IDEncrypted { get; set; }

        public string TitleEN { get; set; }

        public string TitleAR { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string DescriptionEN { get; set; }

        public string DescriptionAR { get; set; }

        public bool Manageable { get; set; }

        public int? DuplicatedFromQuestionGroupID { get; set; }

        public string DuplicatedFromQuestionGroup { get; set; }

        public List<QuestionView> Questions { get; set; }
    }
}
