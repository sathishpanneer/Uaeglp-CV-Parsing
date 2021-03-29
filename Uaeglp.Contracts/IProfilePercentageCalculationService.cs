using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Contracts
{
	public interface IProfilePercentageCalculationService
    {
        Task<decimal> UpdateProfileCompletedPercentageAsync(int profileId);
    }
}
