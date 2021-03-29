using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Uaeglp.Contracts;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.Web.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QRScanController : ControllerBase
    {
        private readonly ILogger<QRScanController> _logger;
        private readonly IQRScanService _service;

        public QRScanController(ILogger<QRScanController> logger, IQRScanService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("get-qrscan-event/{userId}", Name = "GetEventByQrScan")]
        public async Task<IActionResult> ScanQRCodeAsync(int userId, string _data)
        {
            var result = await _service.ScanQRCodeAsync(userId, _data);
            return Ok(result);
        }
        [HttpPut("join-qrscan-event/{userId}/{decisionId}/{_data}", Name = "JoinEvent")]
        public async Task<IActionResult> JoinEventByQRCodeAsync(int userId, int decisionId, string _data)
        {
            var result = await _service.JoinEventByQRCodeAsync(userId, decisionId, _data);
            return Ok(result);
        }

        [HttpPut("join-event-usershortcode/{eventid}/{shortCode}", Name = "JoinEventByUserShortCode")]
        public async Task<IActionResult> JoinAdminEventByShortCodeAsync(int eventid, string shortCode)
        {
            var result = await _service.JoinAdminEventByShortCodeAsync(eventid, shortCode);
            return Ok(result);
        }

        [HttpPut("join-event-userqrcode/{eventid}/{qrCode}", Name = "JoinEventByUserQrCode")]
        public async Task<IActionResult> JoinAdminEventByQRCodeAsync(int eventid, string qrCode)
        {
            var result = await _service.JoinAdminEventByQRCodeAsync(eventid, qrCode);
            return Ok(result);
        }
        [HttpGet("get-userdata-byshortcode/{shortCode}", Name = "GetUserDataByshortCodeAsync")]
        public async Task<IActionResult> GetUserDataByshortCodeAsync(string shortCode)
        {
            var result = await _service.GetUserDataByshortCodeAsync(shortCode);
            return Ok(result);
        }
        [HttpGet("get-userdata-byqrcode/{qrCode}", Name = "GetUserDataByQrCodeAsync")]
        public async Task<IActionResult> GetUserDataByQrCodeAsync(string qrCode)
        {
            var result = await _service.GetUserDataByQrCodeAsync(qrCode);
            return Ok(result);
        }

        [HttpGet("get-admin-registerevents", Name = "GetEventsAsync")]
        public async Task<IActionResult> GetEventsAsync()
        {
            var result = await _service.GetEvents();
            return Ok(result);
        }

        [HttpGet("get-event-participants/{eventid}", Name = "GetEventparticipants")]
        public async Task<IActionResult> GetEventparticipants(int eventid)
        {
            var result = await _service.GetEventparticipants(eventid);
            return Ok(result);
        }
    }
}