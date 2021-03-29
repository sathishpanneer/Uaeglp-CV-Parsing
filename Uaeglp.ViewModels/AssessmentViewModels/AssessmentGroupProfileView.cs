using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentGroupProfileView
	{
		public string ProfileName { get; set; }

		public string GroupName { get; set; }

		public int ProfileID { get; set; }

		public int GroupID { get; set; }

		public List<AssessmentToolReportView> Reports { get; set; }
	}
}
