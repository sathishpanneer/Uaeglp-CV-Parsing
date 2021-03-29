using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Uaeglp.Contracts;
using Uaeglp.ViewModels;

namespace Uaeglp.Web.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ReportProblemController : ControllerBase
    {
        private readonly IReportProblemService _reportProblemService;
        public ReportProblemController(IReportProblemService reportProblemService)
        {
            _reportProblemService = reportProblemService;
        }

        [HttpPost("reportProblem", Name = "ReportProblem")]
        public async Task<IActionResult> ReportProblemAsync([FromForm] ReportProblemView view)
        {
            var data = await _reportProblemService.ReportProblemAsync(view);

            return Ok(data);
        }
    }
}