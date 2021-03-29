using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels
{
	public class ReportView
	{
		public string ID { get; set; }
		public int UserID { get; set; }
		public int ReasonTypeID { get; set; }
		public string Reason { get; set; }
		public DateTime Created { get; set; }
	}
}
