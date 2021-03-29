using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Uaeglp.Contracts;
using Uaeglp.ViewModels.AssessmentViewModels;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.Web.Controllers
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

        [HttpGet("get-assessments-report-by-profile/{profileId}/{AssessmentToolId}", Name = "GetAssessmentsReportByProfile")]
        public async Task<IActionResult> GetAssessmentsReportByProfile(int profileId, int? AssessmentToolId = null)
        {
            var result = await _service.GetReportByProfile(profileId, AssessmentToolId);
            return Ok(result);
        }

        [HttpGet("get-assessments-report-list-by-profile/{profileId}", Name = "GetAssessmentsReportListByProfile")]
        public async Task<IActionResult> GetAssessmentsReportListByProfile(int profileId)
        {
            var result = await _service.GetReportByProfile(profileId, null);
            return Ok(result);
        }

        [HttpGet("get-assessments-report-list-by-profile-group/{profileId}/{groupId}", Name = "GetAssessmentsReportListByProfileGroup")]
        public async Task<IActionResult> GetAssessmentsReportListByProfileGroup(int profileId, int groupId)
        {
            var result = await _service.GetProfileDrillDownByGroup_New(profileId, groupId,null);
            return Ok(result);
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

        [HttpGet("get-assessmentgroups-by-user/{userId}/{isAdmin}", Name = "GetAssessmentGroupsByUser")]
        public async Task<IActionResult> GetAssessmentGroupsByUser(int userId, bool isAdmin = false)
        {
            var result = await _service.GetAssessmentGroupsByUserId(userId, isAdmin);
            return Ok(result);
        }

        [HttpGet("get-assessmentgroup-members/{assessmentGroupid}", Name = "GetMembers")]
        public async Task<IActionResult> GetMembers(int assessmentGroupid)
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
        public async Task<IActionResult> GetAssessmentQuestions(int assessmentId, int profileId, LanguageType language = LanguageType.AR)
        {
            var result = await _service.GetAssessmentQuestions(assessmentId, profileId, language);
            return Ok(result);
        }

        [HttpGet("is-profile-coordinator/{profileId}", Name = "IsCoordinator")]
        public async Task<IActionResult> IsCoordinator(int profileId)
        {
            var result = await _service.IsCoordinator(profileId);
            return Ok(result);
        }

        [HttpGet("set-profile-as-coordinator/{profileId}", Name = "MakeCoordinator")]
        public async Task<IActionResult> MakeCoordinator(int profileId)
        {
            var result = await _service.MakeCoordinator(profileId);
            return Ok(result);
        }

        [HttpGet("remove-profile-as-coordinator/{profileId}", Name = "RemoveCoordinator")]
        public async Task<IActionResult> RemoveCoordinator(int profileId)
        {
            var result = await _service.RemoveCoordinator(profileId);
            return Ok(result);
        }

        [HttpGet("toggle-coordinator/{profileId}/{groupId}", Name = "ToggleCoordintor")]
        public async Task<IActionResult> ToggleCoordintor(int profileId, int groupId)
        {
            var result = await _service.ToggleCoordintor(profileId, groupId);
            return Ok(result);
        }

        [HttpGet("get-questions-with-answers/{profileId}/{paidId}", Name = "GetQuestionsWithAnswers")]
        public async Task<IActionResult> GetQuestionsWithAnswers(int profileId, int paidId)
        {
            var result = await _service.GetQuestionsWithAnswers(paidId, profileId);
            return Ok(result);
        }

        [HttpPost("submit-questions-with-answers/{profileId}", Name = "AnswerFeedBackQuestions")]
        public async Task<IActionResult> AnswerFeedBackQuestions(QuestionWithAnswerView_New questionWithAnswerView, int profileId)
        {
            var result = await _service.AnswerFeedBackQuestions(questionWithAnswerView, profileId);
            return Ok(result);
        }

        [HttpPost("submit-assessment-answers/{assessmentId}/{profileId}/{skip}/{order}", Name = "SubmitAssessmentAnswers")]
        public async Task<IActionResult> SubmitAssessmentAnswers(AssessmentAnswersView assessmentAnswersView, int assessmentId, int profileId, int skip, int order)
        {
            var result = await _service.SubmitAssessmentAnswers(assessmentId, profileId, skip, order, assessmentAnswersView);
            return Ok(result);
        }

        [HttpPost("submit-assessment-head-answers/{assessmentId}/{profileId}/{totalTestCount}/{order}", Name = "SubmitAssessmentHeadAnswers")]
        public async Task<IActionResult> SubmitAssessmentHeadAnswers(AssessmentHeadAnswersView model, 
            int assessmentId, int profileId, int totalTestCount, int order)
        {
            var result = await _service.SubmitAssessmentHeadAnswers(assessmentId, profileId, model.QuestionItemScores, model.AssessmentNavigationObject, totalTestCount, order);
            return Ok(result);
        }

        [HttpPost("submit-assessment-scores/{assessmentId}/{profileId}/{order}/{language}", Name = "SubmitAllScores")]
        public async Task<IActionResult> SubmitAllScores(AssessmentAnswersView assessmentAnswersView, int assessmentId, int profileId, int order, LanguageType language = LanguageType.AR)
        {
            var result = await _service.SubmitAllScores(assessmentId, profileId, order, assessmentAnswersView, language);
            return Ok(result);
        }

        [HttpPost("submit-assessment-head-scores/{assessmentId}/{profileId}/{order}/{language}", Name = "SubmitHeadAllScores")]
        public async Task<IActionResult> SubmitHeadAllScores(List<QuestionScoreView> questionItemScores, int assessmentId, int profileId, int order, LanguageType language = LanguageType.AR)
        {
            var result = await _service.SubmitHeadAllScores(assessmentId, profileId, order, questionItemScores, language);
            return Ok(result);
        }

        [HttpPost("add-assessmentgroup-member/{profileId}/{groupId}", Name = "AddMember")]
        public async Task<IActionResult> AddMember(int profileId, int groupId)
        {
            var result = await _service.AddMember(profileId, groupId);
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