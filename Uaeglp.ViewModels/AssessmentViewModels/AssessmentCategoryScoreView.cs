using System;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentCategoryScoreView
	{
		public int AssessmentID { get; set; }

		public Decimal Score { get; set; }

		public int AssessmenttoolCategoryID { get; set; }

		public DateTime Created { get; set; }
	}
}
