using System.Collections.Generic;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.AssessmentViewModels;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Contracts.Communication
{
	public interface IAccountResponse : IBaseResponse
	{
        UserRegistration User { get; set; }
        UserDeviceInfoViewModel DiviceInfoViewModel { get; set; }
	}
}
