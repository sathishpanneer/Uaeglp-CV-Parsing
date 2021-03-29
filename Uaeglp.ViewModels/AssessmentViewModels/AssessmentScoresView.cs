using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentScoresView
	{
		public Decimal Candidate { get; set; }

		public Decimal Average { get; set; }

		public Decimal Best { get; set; }

		public bool HasOverviewAssessments { get; set; }
	}
}
