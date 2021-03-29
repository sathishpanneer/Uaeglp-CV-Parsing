using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Uaeglp.Contracts;
using Uaeglp.Models;
using Uaeglp.ViewModels.AssessmentViewModels;

namespace Uaeglp.Api.Controllers
{
    [ApiVersion("1.0")]
	[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssessmentController : ControllerBase
    {

        private readonly ILogger<ProfileController> _logger;
        private readonly IAssessmentService _service;

        public AssessmentController(ILogger<ProfileController> logger, IAssessmentService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("get-assessments-by-profile/{profileId}", Name = "GetAssessmentListByProfile")]
        public async Task<IActionResult> GetAssessmentListByProfile(int profileId)
        {
            var result = await _service.GetListByProfile(profileId);
            return Ok(result);
        }

        [HttpGet("get-assessments-by-profile-and-groupid/{profileId}/{groupId}", Name = "GetListByProfileAndGroupID")]
        public async Task<IActionResult> GetListByProfileAndGroupID(int profileId, int groupId)
        {
            var result = await _service.GetListByProfileAndGroupID(profileId, groupId);
            return Ok(result);
        }

        [HttpGet("get-assessmentgroups-by-filter/{userId}/{isAdmin}", Name = "GetAssessmentGroupsByFilter")]
        public async Task<IActionResult> GetAssessmentGroupsByFilter(int userId, bool isAdmin = false)
        {
            var result = await _service.GetAssessmentGroupsByUserId(userId, isAdmin);
            return Ok(result);
        }

        [HttpGet("get-assessmentgroup-members/{assessmentGroupid}", Name = "GetMembers")]
        public async Task<IActionResult> GetMembers(string assessmentGroupid)
        {
            var result = await _service.GetMembers(assessmentGroupid);
            return Ok(result);
        }

        [HttpGet("get-assessment-details/{assessmentId}", Name = "GetAssessmentDetails")]
        public async Task<IActionResult> GetAssessmentDetails(int assessmentId)
        {
            var result = await _service.GetAssessmentDetails(assessmentId);
            return Ok(result);
        }

        [HttpGet("get-assessment-questions/{assessmentId}/{profileId}/{language}", Name = "GetAssessmentQuestions")]
        public async Task<IActionResult> GetAssessmentQuestions(int assessmentId, int profileId, string language)
        {
            var result = await _service.GetAssessmentQuestions(assessmentId, profileId, language);
            return Ok(result);
        }

        [HttpPost("submit-assessment-answers/{assessmentId}/{profileId}/{skip}/{order}", Name = "SubmitAssessmentAnswers")]
        public async Task<IActionResult> SubmitAssessmentAnswers(AssessmentAnswersView assessmentAnswersView, int assessmentId, int profileId, int skip, int order)
        {
            var result = await _service.SubmitAssessmentAnswers(assessmentId, profileId, skip, order, assessmentAnswersView);
            return Ok(result);
        }

        [HttpPost("submit-assessment-scores/{assessmentId}/{profileId}/{order}/{language}", Name = "SubmitAllScores")]
        public async Task<IActionResult> SubmitAllScores(AssessmentAnswersView assessmentAnswersView, int assessmentId, int profileId, int order, string language = "ar")
        {
            var result = await _service.SubmitAllScores(assessmentId, profileId, order, language, assessmentAnswersView);
            return Ok(result);
        }

        [HttpPost("add-assessmentgroup-member/{groupId}", Name = "AddMember")]
        public async Task<IActionResult> AddMember(AssessmentGroupMember member, int groupId)
        {
            var result = await _service.AddMember(member, groupId);
            return Ok(result);
        }

        [HttpDelete("delete-assessmentgroup-member/{groupId}/{memberId}", Name = "DeleteMember")]
        public async Task<IActionResult> DeleteMember(int memberId, int groupId)
        {
            var result = await _service.DeleteMember(memberId, groupId);
            return Ok(result);
        }

        [HttpPost("add-assessmentgroup", Name = "AddAssessmentGroup")]
        public async Task<IActionResult> AddAssessmentGroup(AssessmentGroupView assessmentGroupView)
        {
            var result = await _service.AddAssessmentGroup(assessmentGroupView);
            return Ok(result);
        }

        [HttpPut("update-assessmentgroup", Name = "UpdateAssessmentGroup")]
        public async Task<IActionResult> UpdateAssessmentGroup(AssessmentGroupView assessmentGroupView)
        {
            var result = await _service.UpdateAssessmentGroup(assessmentGroupView);
            return Ok(result);
        }

        [HttpDelete("delete-assessmentgroup/{id}", Name = "DeleteAssessmentGroup")]
        public async Task<IActionResult> DeleteAssessmentGroup(int id)
        {
            var result = await _service.DeleteAssessmentGroup(id);
            return Ok(result);
        }

        [HttpGet("get-assessmentgroup/{id}", Name = "GetAssessmentGroup")]
        public async Task<IActionResult> GetAssessmentGroup(int id)
        {
            var result = await _service.GetAssessmentGroup(id);
            return Ok(result);
        }

        [HttpGet("get-assessment-percentage/{profileId}", Name = "CalculatePercentge")]
        public async Task<IActionResult> CalculatePercentge(int profileId)
        {
            var result = await _service.CalculatePercentge(profileId);
            return Ok(result);
        }
    }
}