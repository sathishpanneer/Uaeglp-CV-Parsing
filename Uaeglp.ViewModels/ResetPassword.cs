using System.ComponentModel.DataAnnotations;

namespace Uaeglp.ViewModels
{
	public class ResetPassword : Result
	{
		[Required]
		public int UserId { get; set; }

		[Required]
		[StringLength(75)]
		public string OldPassword { get; set; }

		[Required]
		[StringLength(75)]
		public string NewPassword { get; set; }
	}
}
