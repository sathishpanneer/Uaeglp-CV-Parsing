using System;
using System.ComponentModel.DataAnnotations;
using Uaeglp.Utilities;

namespace Uaeglp.ViewModels
{
	public class ProfileView
	{
		public int Id { get; set; }

		[Required]
		[StringLength(256)]
		public string FirstName { get; set; }

		[Required]
		[StringLength(256)]
		public string SecondName { get; set; }

		[Required]
		[StringLength(256)]
		public string ThirdName { get; set; }

		[Required]
		[StringLength(256)]
		public string LastName { get; set; }

		public string FirstNameAr { get; set; }
		public string FirstNameEn { get; set; }
		public string LastNameAr { get; set; }
		public string LastNameEn { get; set; }
		public string FullNameEN => FirstNameEn?.Trim() + " " + LastNameEn?.Trim();
		public string FullNameAR => FirstNameAr?.Trim() + " " + LastNameAr?.Trim();
		public string NameEN { get; set; }

		public string NameAR { get; set; }

		public string Email { get; set; }
		public string IDEncrypted { get; set; }
		public bool IsCoordinator { get; set; }
		public Decimal AssessmentCompletionPercentage { get; set; }
		public int Rank { get; set; }
		public int AssessmentCompletionCount { get; set; }
		public int AssessmentTotalCount { get; set; }
		public string DesignationEn { get; set; }
		public string DesignationAr { get; set; }
		public int UserImageFileId { get; set; }
		public string ProfileImageUrl => ConstantUrlPath.ProfileImagePath + UserImageFileId;
		public decimal? TotalScore { get; set; }
	}
}
