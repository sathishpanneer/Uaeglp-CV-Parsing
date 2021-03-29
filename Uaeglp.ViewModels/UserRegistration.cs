using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.ViewModels
{
	public class UserRegistration : Result
	{
		[IgnoreDataMember]
		public int Id { get; set; }
		[Required]
		[StringLength(25)]
		public string FirstName { get; set; }

		[Required]
		[StringLength(25)]
		public string LastName { get; set; }

		[Required]
		[StringLength(256)]
		public string Email { get; set; }

		[Required]
		[StringLength(15)]
		public string EmiratesId{ get; set; }

        public LanguageType LanguageType { get; set; } = LanguageType.EN;

        public bool IsTC_Accepted { get; set; }

        //[IgnoreDataMember]
        //public string Message { get; set; }
    }
}
