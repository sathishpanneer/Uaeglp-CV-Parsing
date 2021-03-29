using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Uaeglp.Contracts;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProgramViewModels;

namespace Uaeglp.Web.Controllers
{
    [ApiVersion("1.0")]
	[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProgramController : Controller
    {
		private readonly ILogger<ProgramController> _logger;
        private readonly IProgramService _programService;

		public ProgramController(ILogger<ProgramController> logger, IProgramService programService)
        {
            _logger = logger;
            _programService = programService;
        }

		// GET: api/Ping
		[HttpGet("completed-program/{userId}", Name = "CompletedProgramList")]
        public async Task<IActionResult> GetCompletedProgramAsync(int userId)
        {
            var data = await _programService.GetCompletedProgramAsync(userId);
            return Ok(data);
        }

        [HttpGet("{profileId}", Name = "GetPrograms")]
        public async Task<IActionResult> GetAllProgramAsync(int profileId)
        {
            var data = await _programService.GetAllProgramAsync(profileId);
            return Ok(data);
        }



        [HttpGet("{profileId}/{batchId}", Name = "GetProgramDetail")]
        public async Task<IActionResult> GetProgramDetailsAsync(int profileId, int batchId)
        {
            var data = await _programService.GetProgramDetailsAsync(profileId, batchId);

            return Ok(data);
        }

        [HttpGet("batch/{profileId}/{batchId}", Name = "GetBatchDetails")]
        public async Task<IActionResult> GetBatchDetailsAsync(int profileId, int batchId)
        {
            var data = await _programService.GetBatchsDetailAsync(profileId,batchId);

            return Ok(data);
        } 
        
        [HttpGet("questions/{profileId}/{batchId}", Name = "GetBatchQuestions")]
        public async Task<IActionResult> GetQuestionsAsync(int profileId, int batchId)
        {
            var data = await _programService.GetBatchQuestionsAsync(profileId,batchId);

            return Ok(data);
        }

        [HttpGet("get-reference-number/{profileId}/{batchId}", Name = "GetProgramReferenceNumnber")]
        public async Task<IActionResult> GetReferenceAsync(int profileId, int batchId)
        {
            var data = await _programService.GetReferenceAsync(profileId, batchId);

            return Ok(data);
        }

        [HttpPost("answers", Name = "AddOrUpdateApplicationAnswerAsync")]
        public async Task<IActionResult> AddOrUpdateApplicationAnswerAsync([FromForm]ProgramAnswerViewModel answerViewModel)
        {
            var data = await _programService.AddOrUpdateApplicationAnswerAsync(answerViewModel);

            return Ok(data);
        }
    }
}
