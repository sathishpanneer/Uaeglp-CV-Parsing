using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Uaeglp.Contracts;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Web.Controllers
{
	[ApiVersion("1.0")]
	[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileAssessmentController : ControllerBase
    {

        private readonly ILogger<ProfileController> _logger;
        private readonly IProfileAssessmentService _service;

        public ProfileAssessmentController(ILogger<ProfileController> logger, IProfileAssessmentService service)
        {
            _logger = logger;
            _service = service;
        }


        [HttpGet("bioinfo/{userId}", Name = "GetBioInfo")]
        public async Task<IActionResult> GetBioInfoAsync(int userId)
        {
            var result = await _service.GetBioInfoAsync(userId);
            return Ok(result);
        }

        [HttpPost("updatebioinfo", Name = "UpdateBioInfo")]
        public async Task<IActionResult> UpdateBioInfoAsync([FromForm]BioVideoView model)
        {
            var result = await _service.UpdateBioInfoAsync(model).ConfigureAwait(false);
            return Ok(result);
        }


        [HttpPost("addorupdate-education", Name = "AddOrUpdateProfileEducation")]
        public async Task<IActionResult> AddOrUpdateProfileEducationAsync([FromBody]ProfileEducationView model)
        {
            var result = await _service.AddOrUpdateProfileEducationAsync(model);
            return Ok(result);
        }



        [HttpDelete("delete-education/{educationId}", Name = "DeleteProfileEducation")]
        public async Task<IActionResult> DeleteProfileEducationAsync(int educationId)
        {
            var result = await _service.DeleteProfileEducationAsync(educationId);
            return Ok(result);
        }

        [HttpPost("addorupdate-workexperience", Name = "AddOrUpdateProfileWorkExperience")]
        public async Task<IActionResult> AddOrUpdateProfileWorkExperienceAsync([FromBody]ProfileWorkExperienceView model)
        {
            var result = await _service.AddOrUpdateProfileWorkExperienceAsync(model);
            return Ok(result);
        }

        [HttpDelete("delete-experience/{experienceId}", Name = "DeleteProfileWorkExperience")]
        public async Task<IActionResult> DeleteProfileWorkExperienceAsync(int experienceId)
        {
            var result = await _service.DeleteProfileWorkExperienceAsync(experienceId);
            return Ok(result);
        }


        [HttpPost("addorupdate-skillandinterest", Name = "AddOrUpdateSkillsAndInterest")]
        public async Task<IActionResult> AddOrUpdateSkillsAndInterestAsync([FromBody]SkillAndInterestView model)
        {
            var result = await _service.AddOrUpdateSkillsAndInterestAsync(model);
            return Ok(result);
        }

        [HttpPost("addorupdate-skill", Name = "AddOrUpdateSkills")]
        public async Task<IActionResult> AddOrUpdateSkillsAsync([FromBody]SkillAndInterestView model)
        {
            var result = await _service.AddOrUpdateSkillsAsync(model);
            return Ok(result);
        }

        [HttpPost("addorupdate-interest", Name = "AddOrUpdateInterest")]
        public async Task<IActionResult> AddOrUpdateInterestAsync([FromBody]SkillAndInterestView model)
        {
            var result = await _service.AddOrUpdateInterestAsync(model);
            return Ok(result);
        }


        [HttpPost("addorupdate-learning-preference/{profileId}", Name = "AddOrUpdateLearningPreference")]
        public async Task<IActionResult> AddOrUpdateLearningPreferenceAsync(int profileId, [FromBody]List<ProfileLearningPreferenceView> model)
        {
            var result = await _service.AddOrUpdateLearningPreferenceAsync(profileId, model);
            return Ok(result);
        }

        [HttpPost("addorupdate-profile-achievement", Name = "AddOrUpdateProfileAchievement")]
        public async Task<IActionResult> AddOrUpdateProfileAchievementAsync([FromBody]ProfileAchievementView model)
        {
            var result = await _service.AddOrUpdateProfileAchievementAsync(model);
            return Ok(result);
        }


        [HttpDelete("delete-achievement/{achievementId}", Name = "DeleteProfileAchievement")]
        public async Task<IActionResult> DeleteProfileAchievementAsync(int achievementId)
        {
            var result = await _service.DeleteProfileAchievementAsync(achievementId);
            return Ok(result);
        }

        [HttpPost("addorupdate-profile-membership", Name = "AddOrUpdateProfileMembership")]
        public async Task<IActionResult> AddOrUpdateProfileMembershipAsync([FromBody]ProfileMembershipView model)
        {
            var result = await _service.AddOrUpdateProfileMembershipAsync(model);
            return Ok(result);
        }

        [HttpDelete("delete-membership/{membershipId}", Name = "DeleteProfileMembership")]
        public async Task<IActionResult> DeleteProfileMembershipAsync(int membershipId)
        {
            var result = await _service.DeleteProfileMembershipAsync(membershipId);
            return Ok(result);
        }

        [HttpPost("addorupdate-profile-training", Name = "AddOrUpdateProfileTraining")]
        public async Task<IActionResult> AddOrUpdateProfileTrainingAsync([FromBody]ProfileTrainingView model)
        {
            var result = await _service.AddOrUpdateProfileTrainingAsync(model);
            return Ok(result);
        }

        [HttpDelete("delete-training/{trainingId}", Name = "DeleteProfileTraining")]
        public async Task<IActionResult> DeleteProfileTrainingAsync(int trainingId)
        {
            var result = await _service.DeleteProfileTrainingAsync(trainingId);
            return Ok(result);
        }
    }
}