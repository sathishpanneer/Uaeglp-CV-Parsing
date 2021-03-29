using System;
using System.Collections.Generic;
using System.Text;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels;

namespace Uaeglp.Services.Communication
{
    public class OTPResponce : Response<ViewModels.ValidateOtp>
	{
		private OTPResponce(bool success, string message, ViewModels.ValidateOtp user) : base(success, message)
		{
			Resource = user;
		}

		/// <summary>
		/// Creates a success response.
		/// </summary>
		/// <param name="user">user view model.</param>
		/// <returns>Response.</returns>
		public OTPResponce(ViewModels.ValidateOtp user) : this(true, string.Empty, user)
		{ }

		/// <summary>
		/// Creates am error response.
		/// </summary>
		/// <param name="Message">Error Message.</param>
		/// <returns>Response.</returns>
		//public OTPResponce(ViewModels.ValidateOtp user, RespCode HTTPStatusCode) : this(false, user)
		//{ }
	}
}
