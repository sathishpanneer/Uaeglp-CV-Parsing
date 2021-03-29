using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Uaeglp.ViewModels
{
	public class ValidateOtp : Result
	{
		[Required]
		public int UserId { get; set; }

		[Required]
		public int? Otp { get; set; }
	}
}
