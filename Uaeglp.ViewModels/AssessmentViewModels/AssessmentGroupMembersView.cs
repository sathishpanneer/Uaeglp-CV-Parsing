using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentGroupMembersView
	{
		public List<int> MembersIDs { get; set; }

		public string MembersIDsString { get; set; }

		public string GroupIDString { get; set; }

		public FileViewModel FileDTO { get; set; }
	}
}
