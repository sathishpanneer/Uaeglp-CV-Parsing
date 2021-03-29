using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uaeglp.Contracts;
using Uaeglp.ViewModels.Enums;
using System;

namespace Uaeglp.Web.Controllers
{
	[ApiVersion("1.0")]
	[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LookupController : ControllerBase
    {
        private readonly ILookupService _lookupService;
        public LookupController(ILookupService lookupService)
        {
            _lookupService = lookupService;
        }

        [HttpGet("get-all-lookupItem-list/{FromDate}", Name = "GetAllLookupItems")]
        public async Task<IActionResult> GetAllLookupItemsAsync(DateTime FromDate)
        {
            var result = await _lookupService.GetAllLookupItemsAsync(FromDate);
            return Ok(result);
        }

        [HttpGet("get-lookupItem-list", Name = "LookupItems")]
        public async Task<IActionResult> GetLookupItemsAsync(LookupType lookupType)
        {
            var result = await _lookupService.GetLookupItemsAsync(lookupType);
            return Ok(result);
        }

        [HttpGet("get-degree-list", Name = "GetDegrees")]
        public async Task<IActionResult> GetDegreeListAsync()
        {
            var result = await _lookupService.GetDegreesAsync();
            return Ok(result);
        }

        [HttpGet("get-interest-list", Name = "GetInterestList")]
        public async Task<IActionResult> GetInterestListAsync()
        {
            var result = await _lookupService.GetInterestListAsync();
            return Ok(result);
        }

        [HttpGet("get-learningpreference-list", Name = "GetProfileLearningPreference")]
        public async Task<IActionResult> GetProfileLearningPreferenceListAsync()
        {
            var result = await _lookupService.GetProfileLearningPreferenceListAsync();
            return Ok(result);
        }

        [HttpGet("get-fieldofstudy-list", Name = "GetEducationFieldOfStudy")]
        public async Task<IActionResult> GetEducationFieldOfStudyAsync()
        {
            var result = await _lookupService.GetEducationFieldOfStudyAsync();
            return Ok(result);
        }

        [HttpGet("get-jobtitle-list", Name = "WorkExperienceJobTitle")]
        public async Task<IActionResult> GetWorkExperienceJobTitleAsync()
        {
            var result = await _lookupService.GetWorkExperienceJobTitleAsync();
            return Ok(result);
        }


        [HttpGet("get-workfield-list", Name = "WorkField")]
        public async Task<IActionResult> GetWorkFieldAsync()
        {
            var result = await _lookupService.GetWorkFieldAsync();
            return Ok(result);
        }

        //WorkField

        [HttpGet("get-organization-list", Name = "GetOrganizationList")]
        public async Task<IActionResult> GetOrganizationListAsync()
        {
            var result = await _lookupService.GetGlpOrganizationListAsync();
            return Ok(result);
        }

        [HttpGet("get-country-list", Name = "GetCountryList")]
        public async Task<IActionResult> GetCountryListAsync()
        {
            var result = await _lookupService.GetCountryListAsync();
            return Ok(result);
        }

        [HttpGet("get-industry-list", Name = "GetIndustryList")]
        public async Task<IActionResult> GetIndustryListAsync()
        {
            var result = await _lookupService.GetIndustryListAsync();
            return Ok(result);
        }

        [HttpGet("get-profileskills", Name = "GetProfileSkills")]
        public async Task<IActionResult> GetProfileSkillsAsync()
        {
            var result = await _lookupService.GetProfileSkillsAsync();
            return Ok(result);
        }


    }
}