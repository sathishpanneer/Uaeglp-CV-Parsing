using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;
using Uaeglp.ViewModels.ProgramViewModels;

namespace Uaeglp.Contracts
{
	public interface IApplicationProgressStatusService
    {
        Task<bool> CreateApplicationProgressAsync(int applicationId, int profileId);
        Task UpdateApplicationProgressAsync(int applicationId, int profileId, int batchId);

        Task<ProfileCompletedViewModel> GetProfileCompletedDetailsAsync(int profileId);

        Task<ProgramCompletedDetailsViewModel> GetProgramCompletedDetailsAsync(int profileId, int batchId);
    }
}
