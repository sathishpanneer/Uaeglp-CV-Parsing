using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Uaeglp.Contracts;
using Uaeglp.ViewModels.ActivityViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProgramViewModels;

namespace Uaeglp.Api.Controllers
{
    [ApiVersion("1.0")]
	[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ActivityAndChallengeController : Controller
    {
		private readonly ILogger<ProgramController> _logger;
        private readonly IActivityAndChallengesService _service;

		public ActivityAndChallengeController(ILogger<ProgramController> logger, IActivityAndChallengesService service)
        {
            _logger = logger;
            _service = service;
        }

		[HttpGet("category/{profileId}", Name = "GetActivityCategoryAsync")]
        public async Task<IActionResult> GetActivityCategoryAsync(int profileId)
        {
            var data = await _service.GetActivityCategoryAsync(profileId);
            return Ok(data);
        }

        [HttpGet("activity/{profileId}/{categoryId}", Name = "GetActivityListAsync")]
        public async Task<IActionResult> GetActivityListAsync(int profileId, int categoryId)
        {
            var data = await _service.GetActivityListAsync(profileId, categoryId);
            return Ok(data);
        }

        [HttpGet("questions/{profileId}/{initiativeId}", Name = "GetActivityQuestionsAsync")]
        public async Task<IActionResult> GetActivityQuestionsAsync(int profileId, int initiativeId)
        {
            var data = await _service.GetActivityQuestionsAsync(profileId,initiativeId);

            return Ok(data);
        }

        [HttpPost("answers", Name = "AddActivityAnswerAsync")]
        public async Task<IActionResult> AddActivityAnswerAsync(ActivityAnswerViewModel model)
        {
            var data = await _service.AddActivityAnswerAsync(model);

            return Ok(data);
        } 
      
    }
}
