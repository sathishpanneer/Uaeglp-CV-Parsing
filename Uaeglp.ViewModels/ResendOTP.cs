using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.ViewModels
{
    public class ResendOTP
    {
		[Required]
		public int UserId { get; set; }

		[IgnoreDataMember]
		public int StatusCode { get; set; }

		[IgnoreDataMember]
		public string Message { get; set; }

		public ResendOtpType ResendOtpType { get; set; }

        public LanguageType LanguageType { get; set; } = LanguageType.EN;
    }
}
