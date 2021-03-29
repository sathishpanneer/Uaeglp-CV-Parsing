using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Uaeglp.ViewModels
{
	public class Result
	{
		[IgnoreDataMember]
		public bool Success { get; set; }

		[IgnoreDataMember]
		public CustomStatusCode CustomStatusCode { get; set; }

		[IgnoreDataMember]
		public string Message { get; set; }

	}
}
