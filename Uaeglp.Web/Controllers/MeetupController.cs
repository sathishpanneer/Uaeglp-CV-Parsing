using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Uaeglp.Contracts;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.Meetup;

namespace Uaeglp.Web.Controllers
{
    [ApiVersion("1.0")]
	[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MeetupController : ControllerBase
    {
        private readonly ILogger<MeetupController> _logger;
        private readonly IMeetupService _service;

        public MeetupController(ILogger<MeetupController> logger, IMeetupService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("get-groups-by-filters/{userId}", Name = "GetGroupsByFilters")]
        public async Task<IActionResult> GetGroupsByFilters(PagingView pagingView, int userId, [FromQuery] string SearchText = null)
        {
            var result = await _service.GetGroupsByFilters(pagingView, userId, SearchText);
            return Ok(result);
        }

        [HttpGet("get-groups/{userId}/{limit}", Name = "GetGroups")]
        public async Task<IActionResult> GetGroups(int userId,int limit)
        {
            var result = await _service.GetGroups(userId, limit);
            return Ok(result);
        }

        [HttpGet("get-group/{id}/{userId}", Name = "GetGroup")]
        public async Task<IActionResult> GetGroup(int id, int userId)
        {
            var result = await _service.GetGroup(id, userId);
            return Ok(result);
        }

        [HttpPost("add-group/{userId}", Name = "CreateGroup")]
        public async Task<IActionResult> CreateGroup(int userId, GroupView groupView)
        {
            var result = await _service.CreateGroup(userId, groupView);
            return Ok(result);
        }

        [HttpPut("update-group/{userId}", Name = "UpdateGroup")]
        public async Task<IActionResult> UpdateGroup(int userId, GroupView groupView)
        {
            var result = await _service.UpdateGroup(userId, groupView);
            return Ok(result);
        }

        [HttpPut("follow-group/{userId}", Name = "FollowGroup")]
        public async Task<IActionResult> FollowGroup(int groupId, int userId)
        {
            var result = await _service.FollowGroup(groupId, userId);
            return Ok(result);
        }

        [HttpPut("unfollow-group/{userId}", Name = "UnfollowGroup")]
        public async Task<IActionResult> UnfollowGroup(int groupId, int userId)
        {
            var result = await _service.UnFollowGroup(groupId, userId);
            return Ok(result);
        }
       
        [HttpGet("get-meetups-by-groupid/{groupId}/{userId}/{skip}/{limit}", Name = "GetMeetupsByGroupId")]
        public async Task<IActionResult> GetMeetupsByGroupId(int groupId, int userId, int skip, int limit)
        {
            var result = await _service.GetMeetupsByGroupId(groupId, userId, skip, limit);
            return Ok(result);
        }

        [HttpGet("get-meetup-profile-by-groupid/{groupId}/{userId}/{skip}/{limit}", Name = "GetMeetupsProfileByGroupId")]
        public async Task<IActionResult> GetMeetupsProfileByGroupId(int groupId, int userId, int skip, int limit)
        {
            var result = await _service.GetMeetupsProfileByGroupId(groupId, userId, skip, limit);
            return Ok(result);
        }

        [HttpGet("get-meetup/{id}/{userId}", Name = "GetMeetup")]
        public async Task<IActionResult> GetMeetup(int id, int userId)
        {
            var result = await _service.GetMeetup(id, userId);
            return Ok(result);
        }
       
        [HttpPost("add-meetup", Name = "CreateMeetup")]
        public async Task<IActionResult> CreateMeetup([FromForm]MeetupAdd meetup)
        {
            var result = await _service.CreateMeetup(meetup);
            return Ok(result);
        }
       
        [HttpPut("update-meetup", Name = "UpdateMeetup")]
        public async Task<IActionResult> UpdateMeetup([FromForm]MeetupAdd meetup)
        {
            var result = await _service.UpdateMeetup(meetup);
            return Ok(result);
        }

        [HttpDelete("delete-meetup/{id}", Name = "DeleteMeetup")]
        public async Task<IActionResult> DeleteMeetup(int id)
        {
            var result = await _service.DeleteMeetup(id);
            return Ok(result);
        }

        [HttpPut("set-meetup-decision/{decisionId}/{meetupId}/{userId}", Name = "SetMeetupDesicion")]
        public async Task<IActionResult> SetMeetupDesicion(int decisionId, int meetupId, int userId)
        {
            var result = await _service.SetMeetupDesicion(decisionId, meetupId, userId);
            return Ok(result);
        }

        [HttpPost("add-meetup-comments", Name = "CreateComments")]
        public async Task<IActionResult> AddMeetupCommendsAsync([FromForm]MeetupCommendAddView view)
        {
            var result = await _service.AddMeetupCommendsAsync(view);
            return Ok(result);
        }

        [HttpGet("get-meetup-comments/{meetupid}/{userid}", Name = "GetComments")]
        public async Task<IActionResult> GetMeetupCommentsAsync(int meetupid,int userid, int Skip, int Take)
        {
            var result = await _service.GetMeetupCommentsAsync(meetupid, userid,  Skip,  Take);
            return Ok(result);
        }
        [HttpPut("edit-meetup-comments/{commentid}", Name = "EditComments")]
        public async Task<IActionResult> EditMeetupCommentAsync([FromForm]MeetupCommendAddView view, string commentid)
        {
            var result = await _service.EditMeetupCommentAsync(view, commentid);
            return Ok(result);
        }
        [HttpDelete("delete-meetup-comments/{commentid}", Name = "DeleteComments")]
        public async Task<IActionResult> DeleteMeetupCommentAsync(int meetupid, string commentid, int userid)
        {
            var result = await _service.DeleteMeetupCommentAsync(meetupid, commentid, userid);
            return Ok(result);
        }
        [HttpPut("generate-meetup-qrcode/{meetupid}/{userId}", Name = "GenerateQrCodeMeetup")]
        public async Task<IActionResult> GenerateMeetupQRCodeAsync(int meetupid, int userId)
        {
            var result = await _service.GenerateMeetupQRCodeAsync(meetupid, userId);
            return Ok(result);
        }
    }
}