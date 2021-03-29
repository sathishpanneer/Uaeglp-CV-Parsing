using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentGrouping
	{
		public int? ID { get; set; }

		public string Name { get; set; }

		public bool HasSubScale { get; set; }

		public List<FactorGroupingView> Factors { get; set; }
	}
}
