using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class ProfileAssessmentQuestionView
	{
        public List<QuestionItemScoreView> QuestionItemScores { get; set; }

        public int AssessmentToolID { get; set; }

        public int AssessmentToolTypeID { get; set; }

        public int AssessmentToolCategoryID { get; set; }

        public string AssessmentToolName { get; set; }

        public int ProfileID { get; set; }

        public int SkippedCount { get; set; }

        public int TotalTestCount { get; set; }

        public bool IsLastCount { get; set; }

        public bool HasSubScale { get; set; }

        public bool IsCompleted { get; set; }

        public string QuestionIDs { get; set; }

        public bool HasQuestionDirect { get; set; }

        public int Order { get; set; }
    }
}
