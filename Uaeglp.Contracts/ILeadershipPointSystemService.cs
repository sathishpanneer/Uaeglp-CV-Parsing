using System;
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
	public interface ILeadershipPointSystemService
    {
        Task<ILeadershipPointSystemResponse> GetLeadershipPointSystemAsync(int profileId);
        Task<ILeadershipPointSystemResponse> GetCriteriaClaimedPointsAsync(int profileId, int criteriaId);
        Task<ILeadershipPointSystemResponse> GetCriteriaMoreDetailsAsync(Guid? correlationId);
        Task<ILeadershipPointSystemResponse> AddCriteriaClaimAsync(CriteriaClaimRequestView model);
    }
}
