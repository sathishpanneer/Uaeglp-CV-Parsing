using System;
using System.Collections.Generic;
using Uaeglp.Utilities;

namespace Uaeglp.ViewModels.AssessmentViewModels
{
	public class AssessmentGroupView
	{
		public string NameEN { get; set; }

		public string NameAR { get; set; }

		public DateTime DateFrom { get; set; }

		public DateTime DateTo { get; set; }

		public string Color { get; set; }

		public int AssessmentToolMatrixID { get; set; }

		public int? LogoID { get; set; }

		public int ID { get; set; }

		public string IDEncrypted { get; set; }

		public string LogoIDEncrypted { get; set; }
		public List<ProfileView> Members { get; set; }
		public int MembersCount { get; set; }

		public FileViewModel LogoFileDTO { get; set; }
		public string LogoImageUrl => (LogoID == null)? ConstantUrlPath.ProfileImagePath + 0: ConstantUrlPath.ProfileImagePath + LogoID.ToString();
	}
}
