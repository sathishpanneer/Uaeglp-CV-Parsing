using System.Collections.Generic;
using System.Threading.Tasks;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;
using System;

namespace Uaeglp.Contracts
{
	public interface ILookupService
    {
        Task<ILookupResponse> GetAllLookupItemsAsync(DateTime FromDate);
        Task<ILookupResponse> GetLookupItemsAsync(LookupType lookupType);
        Task<ILookupResponse> GetDegreesAsync();
        Task<ILookupResponse> GetInterestListAsync();
        Task<ILookupResponse> GetProfileLearningPreferenceListAsync();
        Task<ILookupResponse> GetEducationFieldOfStudyAsync();
        Task<ILookupResponse> GetGlpOrganizationListAsync();
        Task<ILookupResponse> GetCountryListAsync();
        Task<ILookupResponse> GetIndustryListAsync();
        Task<ILookupResponse> GetProfileSkillsAsync();
        Task<ILookupResponse> GetWorkExperienceJobTitleAsync();
        Task<ILookupResponse> GetWorkFieldAsync();
    }
}
