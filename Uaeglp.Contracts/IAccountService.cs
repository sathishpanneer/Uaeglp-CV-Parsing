using System;
using System.Threading.Tasks;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Contracts
{
	public interface IAccountService
	{
		Task<IAccountResponse> SignupAsync(ViewModels.UserRegistration view);
		Task<IAccountResponse> ResetPassword(ViewModels.ResetPassword view);
		Task<IAccountResponse> SetNewPassword(ViewModels.SetNewPassword view);
		Task<IResponse<ViewModels.ValidateOtp>> ValidateOtp(ViewModels.ValidateOtp view);

		Task<IResponse<ViewModels.ValidateOtp>> ResendOtp(ViewModels.ResendOTP view);

		Task<IAccountResponse> ForgotPass(ViewModels.ForgotPassword view);

		Task<IAccountResponse> ForgotEmailID(ViewModels.ForgotEmail view);
        Task<IAccountResponse> AddOrUpdateDeviceInfoAsync(UserDeviceInfoViewModel model);

		Task<LoginResponseViewModel> LoginAsync(AuthenticateViewModel model);

        Task<bool> LogOutAsync(int userId, string deviceId);


		Task<IResponse<UserDetailsView>> GetUserDetailsAsync(int userId);

    }
}
