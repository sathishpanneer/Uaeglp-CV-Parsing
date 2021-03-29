using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Uaeglp.Contracts;
using Uaeglp.Models;
using Uaeglp.ViewModels;

namespace Uaeglp.Web.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserRecommendationController : ControllerBase
    {
        private readonly IUserRecommendationService _userRecommendationService;
        public UserRecommendationController(IUserRecommendationService userRecommendationService)
        {
            _userRecommendationService = userRecommendationService;
        }

        [HttpPost("sendRecommendation", Name = "SendRecommendation")]
        public async Task<IActionResult> SendRecommendationAsync([FromBody] MultipleUserRecommendationModelView view)
        {
            var result = await _userRecommendationService.SendRecommendationAsync(view);
            return Ok(result);
        }

        [HttpPut("accept-declineRecommendation/{recommendId}/{recipientUserId}/{isAccepted}/{isDeclined}", Name = "AcceptDeclineRecommendation")]
        public async Task<IActionResult> AcceptDeclineRecommendationAsync(int recommendId, int recipientUserId, bool isAccepted, bool isDeclined)
        {
            var result = await _userRecommendationService.AcceptDeclineRecommendationAsync(recommendId, recipientUserId, isAccepted, isDeclined);
            return Ok(result);
        }

        [HttpGet("receiveRecommendation/{recipientUserId}/{recommendID}", Name = "ReceiveRecommendation")]
        private async Task<IActionResult> ReceiveRecommendationAsync(int recipientUserId, int recommendID)
        {
            var data = await _userRecommendationService.ReceiveRecommendationAsync(recipientUserId, recommendID);

            return Ok(data);
        }

        [HttpGet("receiveAllRecommendationList/{recipientUserId}", Name = "ReceiveAllRecommendation")]
        public async Task<IActionResult> ReceiveAllRecommendationAsync(int recipientUserId)
        {
            var data = await _userRecommendationService.ReceiveAllRecommendationAsync(recipientUserId);

            return Ok(data);
        }

        [HttpGet("receiveAcceptedAllRecommendationList/{recipientUserId}", Name = "ReceiveAcceptedAllRecommendation")]
        public async Task<IActionResult> ReceiveAcceptedAllRecommendationAsync(int recipientUserId)
        {
            var data = await _userRecommendationService.ReceiveAcceptedAllRecommendationAsync(recipientUserId);

            return Ok(data);
        }
        
        [HttpDelete("delete-recommendation/{recommendId}", Name = "DeleteRecommendation")]
        public async Task<IActionResult> DeleteMeetup(int recommendId)
        {
            var result = await _userRecommendationService.DeleteRecommendationAsync(recommendId);
            return Ok(result);
        }

        [HttpPut("read-recommendation/{recommendId}/{recipientUserId}/{isread}", Name = "ReadRecommendation")]
        public async Task<IActionResult> AcceptDeclineRecommendationAsync(int recommendId, int recipientUserId, bool isread)
        {
            var result = await _userRecommendationService.SetReadRecommendationAsync(recommendId, recipientUserId, isread);
            return Ok(result);
        }
    }
}