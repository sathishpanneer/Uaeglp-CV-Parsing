using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Services.Communication
{
	public class AccountResponse : BaseResponse, IAccountResponse
	{
        public UserRegistration User { get; set; }
        public UserDeviceInfoViewModel DiviceInfoViewModel { get; set; }

		private AccountResponse(bool success, string message, UserRegistration userRegistration) : base(success, message)
        {
            User = userRegistration;
        }

        private AccountResponse(bool success, string message, UserRegistration userRegistration, Exception e) : base(success, message, e)
        {
            User = userRegistration;
        }

        private AccountResponse(bool success, string message, UserDeviceInfoViewModel diviceInfoViewModel) : base(success, message)
        {
            DiviceInfoViewModel = diviceInfoViewModel;
        }

        private AccountResponse(bool success, string message, UserDeviceInfoViewModel diviceInfoViewModel, Exception e) : base(success, message, e)
        {
            DiviceInfoViewModel = diviceInfoViewModel;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="user">user view model.</param>
        /// <returns>Response.</returns>
        public AccountResponse(ViewModels.UserRegistration user) : this(true, string.Empty, user)
        {

        }

        public AccountResponse(ViewModels.UserRegistration user, Exception e) : this(true, string.Empty, user)
        {

        }

        public AccountResponse(ViewModels.UserDeviceInfoViewModel user) : this(true, string.Empty, user)
        {

        }

        public AccountResponse(ViewModels.UserDeviceInfoViewModel user, Exception e) : this(true, string.Empty, user)
        {

        }

        public AccountResponse(string message, HttpStatusCode status) : base(false, message, status)
        { }
        public AccountResponse(Exception e) : base(e)
        { }
        public AccountResponse() : base()
        { }


	}

    public class UserResponse : Response<UserDetailsView>
    {

        public UserResponse(bool success, string message, UserDetailsView user) : base(success, message)
		{
            Resource = user;
		}
        public UserResponse(UserDetailsView user) : this(true, string.Empty, user)
		{
            Resource = user;
        }

	}
}
