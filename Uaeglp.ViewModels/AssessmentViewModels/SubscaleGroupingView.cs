using System;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class SubscaleGroupingView
	{
		public int? SubscaleID { get; set; }

		public string SubscaleName { get; set; }

		public Decimal Mean { get; set; }

		public Decimal StandarDeviation { get; set; }
	}
}
