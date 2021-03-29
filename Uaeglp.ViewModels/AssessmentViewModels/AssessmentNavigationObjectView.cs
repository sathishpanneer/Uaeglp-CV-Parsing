namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentNavigationObjectView
	{
		public int QuestionHeadID { get; set; }
		public int QuestionID { get; set; }
		public int BlockID { get; set; }
		public int SubAssessmentToolID { get; set; }

        public AssessmentNavigationObjectView()
        {
        }

        public AssessmentNavigationObjectView(
          int blockID,
          int subAssessmentToolID,
          int questionHeadID,
          int questionID)
        {
            this.BlockID = blockID;
            this.SubAssessmentToolID = subAssessmentToolID;
            this.QuestionHeadID = questionHeadID;
            this.QuestionID = questionID;
        }
    }
}
