using System.Collections.Generic;
using Uaeglp.Contract.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;

namespace Uaeglp.Contracts.Communication
{
	public interface IAppSettingResponse : IBaseResponse
	{
        List<ApplicationSettingViewModel> ApplicationSettings { get; set; }
	}
}
