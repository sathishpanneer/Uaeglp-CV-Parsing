using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.ViewModels.AssessmentViewModels;

namespace Uaeglp.ViewModels
{
	public class ScaleGroupingView
	{
		public int? ScaleID { get; set; }

		public string ScaleName { get; set; }

		public Decimal Mean { get; set; }

		public Decimal StandarDeviation { get; set; }

		public List<SubscaleGroupingView> SubscaleGroups { get; set; }
	}
}
