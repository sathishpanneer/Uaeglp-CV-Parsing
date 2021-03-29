using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
	public class FactorGroupingView
	{
		public int? FactorID { get; set; }

		public string FactorName { get; set; }

		public Decimal Mean { get; set; }

		public Decimal StandarDeviation { get; set; }

		public List<ScaleGroupingView> ScaleGroups { get; set; }
	}
}
