using System.Collections.Generic;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class QuestionView
	{
        public int ID { get; set; }

        public int QuestionGroupID { get; set; }

        public string IDEncrypted { get; set; }

        public string TextEN { get; set; }

        public string TextAR { get; set; }

        public string Text { get; set; }

        public int QuestionTypeItemID { get; set; }

        public int? PredefinedQuestionID { get; set; }

        public bool IsPredefined { get; set; }

        public string QuestionType { get; set; }

        public int? WordCount { get; set; }

        public int NumberOfGroups { get; set; }

        public List<QuestionGroupView> QuestionGroups { get; set; }

        public List<OptionView> Options { get; set; }

        public int? Order { get; set; }

        public int? PredefinedOrder { get; set; }

        public QuestionView()
        {
            this.QuestionGroups = new List<QuestionGroupView>();
            this.Options = new List<OptionView>();
            this.WordCount = new int?();
        }
    }
}
