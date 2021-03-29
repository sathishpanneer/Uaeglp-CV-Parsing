using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Uaeglp.Contracts;
using Uaeglp.Models;
using Uaeglp.ViewModels;

namespace Uaeglp.Web.Controllers
{
    [ApiVersion("1.0")]
	[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppSettingController : ControllerBase
    {

        private readonly ILogger<ProfileController> _logger;
        private readonly IAppSettingService _service;

        public AppSettingController(ILogger<ProfileController> logger, IAppSettingService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet(Name = "GetAppSetting")]
        public async Task<IActionResult> GetAppSetting()
        {
            var result = await _service.GetAppSettingAsync();
            return Ok(result);
        }

        [HttpPost("update",Name = "PostAppSettingListByProfile")]
        public async Task<IActionResult> UpdateAppSetting(ApplicationSettingViewModel appSetting)
        {
            var result = await _service.UpdateAppSettingAsync(appSetting);
            return Ok(result);
        }

        [HttpPost("insert-entries" ,Name = "InsertAppSetting")]
        public async Task<IActionResult> InsertAppSettingAsync([FromBody] List<ApplicationSettingViewModel> models)
        {
            var result = await _service.InsertAppSettingAsync(models);
            return Ok(result);
        }
    }
}