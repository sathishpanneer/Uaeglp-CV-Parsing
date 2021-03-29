using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.ViewModels
{
	public class ForgotPassword
    {
		[Required]
		[StringLength(256)]
		public string Email { get; set; }

		[IgnoreDataMember]
		public int StatusCode { get; set; }

		[IgnoreDataMember]
		public string Message { get; set; }

        public LanguageType LanguageType { get; set; } = LanguageType.EN;
	}

	public class ForgotEmail
	{
		[Required]
		[StringLength(256)]
		public string EmirateID { get; set; }

		[IgnoreDataMember]
		public int StatusCode { get; set; }

		[IgnoreDataMember]
		public string Message { get; set; }
	}
}
