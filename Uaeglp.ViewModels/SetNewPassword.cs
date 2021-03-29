using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Uaeglp.ViewModels
{
	public class SetNewPassword
	{
		[Required]
		public int UserId { get; set; }

		[Required]
		[StringLength(75)]
		public string NewPassword { get; set; }

		[IgnoreDataMember]
		public int StatusCode { get; set; }

		[IgnoreDataMember]
		public string Message { get; set; }
	}
}
