using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Uaeglp.Services.Nlog;

namespace Uaeglp.Api.Controllers
{
    [ApiVersion("1.0")]
	[Route("api/[controller]")]
    [ApiController]
    public class PingController : Controller
    {
		private readonly ILogger<PingController> _logger;
		private readonly Contracts.IPingService _service;
		private ILog _nLog;

		public PingController(ILogger<PingController> logger, Contracts.IPingService service, ILog nLog)
		{
			_logger = logger;
			_service = service;
			_nLog = nLog;
		}

		// GET: api/Ping
		[HttpGet]
        public IEnumerable<string> Get()
        {
			var str = _service.ping();
			_logger.LogInformation("ping info");
			_nLog.Information("Information is logged");
			return new string[] { "api", "working" };
        }
    }
}
