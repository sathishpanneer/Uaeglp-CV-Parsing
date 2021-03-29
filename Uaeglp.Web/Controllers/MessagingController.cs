using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Uaeglp.Contracts;
using Uaeglp.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Uaeglp.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagingController : Controller
    {
        private readonly IMessagingService _messagingService;
        public MessagingController(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        [HttpGet("room/{userId}/get-room-list", Name = "GetRoomList")]
        public async Task<IActionResult> GetRoomListAsync(int userId)
        {
            var result = await _messagingService.GetRoomListAsync(userId);

            return Ok(result);
        }

        [HttpGet("room/{searchText}/{userId}/search-message", Name = "SearchMessage")]
        public async Task<IActionResult> GetSearchMessage(string searchText, int userId)
        {
            var result = await _messagingService.GetSearchMessage(searchText, userId);

            return Ok(result);
        }

        [HttpGet("users/{searchName}/get-recepient", Name = "SearchRecepient")]
        public async Task<IActionResult> GetRecepientName(string searchName)
        {
            var result = await _messagingService.GetRecepientName(searchName);

            return Ok(result);
        }

        [HttpGet("room/{roomId}/get-room", Name = "GetRoom")]
        public async Task<IActionResult> GetRoomAsync(string roomId)
        {
            var result = await _messagingService.GetRoomAsync(roomId);

            return Ok(result);
        }

        [HttpGet("room/{roomId}/get-room-message", Name = "GetRoomMessage")]
        public async Task<IActionResult> GetRoomMessageAsync(string roomId)
        {
            var result = await _messagingService.GetRoomMessageAsync(roomId);

            return Ok(result);
        }
        [HttpPost("room/add-room", Name = "AddRoom")]
        public async Task<IActionResult> AddRoomAsync([FromBody]AddRoomView view)
        {
            var result = await _messagingService.AddRoomAsync(view);
            return Ok(result);
        }

        [HttpDelete("room/{roomId}/delete-room", Name = "DeleteRoom")]
        public async Task<IActionResult> DeleteRoomAsync(string roomId)
        {
            var result = await _messagingService.DeleteRoomAsync(roomId);

            return Ok(result);
        }

        [HttpPost("room/{roomId}/{userId}/add-room-member", Name = "AddRoomMember")]
        public async Task<IActionResult> AddRoomMemberAsync(string roomId, int userId)
        {
            var result = await _messagingService.AddRoomMemberAsync(roomId, userId);
            return Ok(result);
        }

        [HttpDelete("room/{roomId}/{userId}/delete-room-member", Name = "DeleteRoomMember")]
        public async Task<IActionResult> DeleteRoomMemberAsync(string roomId, int userId)
        {
            var result = await _messagingService.DeleteRoomMemberAsync(roomId, userId);

            return Ok(result);
        }

        [HttpPost("room/add-room-message", Name = "AddRoomMessage")]
        public async Task<IActionResult> AddRoomMessageAsync([FromForm]MessageAddView message)
        {
            var result = await _messagingService.AddRoomMessageAsync(message);
            return Ok(result);
        }

        [HttpDelete("room/{roomId}/{messageId}/delete-room-message", Name = "DeleteRoomMessage")]
        public async Task<IActionResult> DeleteRoomMessageAsync(string roomId, string messageId)
        {
            var result = await _messagingService.DeleteRoomMessageAsync(roomId, messageId);

            return Ok(result);
        }
        [HttpPost("room/new-room-message", Name = "NewRoomMessage")]
        public async Task<IActionResult> CreateRoomAndMessageAsync([FromForm]RoomAndMessageCreateView message)
        {

            var result = await _messagingService.CreateRoomAndMessageAsync(message);
            return Ok(result);
        }

        [HttpPut("room/update-room", Name = "UpdateRoom")]
        public async Task<IActionResult> AcceptDeclineRecommendationAsync([FromBody]UpdateRommView view)
        {
            var result = await _messagingService.UpdateRoomAsync(view);
            return Ok(result);
        }

        [HttpPut("room/read-unread-message/{roomId}/{userId}/{isread}", Name = "ReadUnReadMessage")]
        public async Task<IActionResult> AcceptDeclineRecommendationAsync(string roomId, string messageId, int userId, bool isread)
        {
            var result = await _messagingService.SetReadMessageAsync( roomId,  messageId,  userId,  isread);
            return Ok(result);
        }

        [HttpGet("room/get-notification-count/{userId}", Name = "GetNotificationCount")]
        public async Task<IActionResult> GetNotificationCountAsync(int userId)
        {
            var result = await _messagingService.GetNotificationCount(userId);

            return Ok(result);
        }


    }
}
