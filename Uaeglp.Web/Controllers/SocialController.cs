using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Uaeglp.Web.Extensions;
using Uaeglp.MongoModels;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.PostViewModels;
using Uaeglp.ViewModels.SurveyViewModels;

namespace Uaeglp.Web.Controllers
{
	[ApiVersion("1.0")]
	[Route("/api/[controller]")]
    [ApiController]
    [Authorize]
	public class SocialController : Controller
    {
		private readonly ILogger<SocialController> _logger;
		private readonly Contracts.ISocialService _service;

		public SocialController(ILogger<SocialController> logger, Contracts.ISocialService service)
		{
			_logger = logger;
			_service = service;
		}

		//[HttpPost("add-profile", Name = "AddProfile")]
		//public async Task<IActionResult> AddProfileAsync([FromBody] ViewModels.Profile view)
		//{
		//	if (!ModelState.IsValid)
		//		return BadRequest(ModelState.GetErrorMessages());

		//	var result = await _service.AddProfileAsync(view);
		//	if (!result.Success)
		//		return BadRequest(result);

		//	return Ok(result.Resource);
		//}

		[HttpPost("get/users/posts", Name = "GetPosts")]
		public async Task<IActionResult> GetPostsAsync(GetPostsViewModel postsViewModel)
		{
			var result = await _service.GetPostsAsync(postsViewModel);
            return Ok(result);
		}

        [HttpGet("get-post/{postId}", Name = "GetPost")]
        public async Task<IActionResult> GetPostAsync(string postId, int userId)
        {
            var result = await _service.GetPostAsync(postId, userId);
            return Ok(result);
        }

		[HttpGet("users/{userId}/Filter", Name = "GetFilterType")]
		public async Task<IActionResult> GetFilterTypeAsync(int userId)
		{
			var result = await _service.GetFilterTypeAsync(userId);
		
			return Ok(result);
		}

		[HttpGet("users/{userId}/my-posts", Name = "GetMyPosts")]
		public async Task<IActionResult> GetMyPostAsync(int userId, [FromQuery] int skip = 0, [FromQuery] int limit = 5)
		{
			var result = await _service.GetMyPostsAsync(userId, skip, limit);
		
			return Ok(result);
		}

		[HttpGet("users/{userId}/posts/admin-created", Name = "GetPostAdminCreated")]
		public async Task<IActionResult> GetPostAdminCreatedAsync(int userId, [FromQuery] int skip = 0, [FromQuery] int limit = 5)
		{
			var result = await _service.GetPostsByAdminCreatedAsync(userId, skip, limit);
		
			return Ok(result);
		}

		[HttpGet("users/{userId}/posts/group-created", Name = "GetPostGroupCreated")]
		public async Task<IActionResult> GetPostGroupCreatedAsync(int userId, [FromQuery] int skip = 0, [FromQuery] int limit = 5)
		{
			var result = await _service.GetPostsByGroupCreatedAsync(userId, skip, limit);
			
			return Ok(result);
		}

		[HttpGet("users/{userId}/posts/sorted-by-date-ascending", Name = "GetPostSortedByDateAscending")]
		public async Task<IActionResult> GetPostSortedByDateAscendingAsync(int userId, [FromQuery] int skip = 0, [FromQuery] int limit = 5)
		{
			var result = await _service.GetPostsSortedByDateAscendingAsync(userId, skip, limit);
			
			return Ok(result);
		}

		[HttpGet("users/{userId}/posts/sorted-by-date-descending", Name = "GetPostSortedByDateDescending")]
		public async Task<IActionResult> GetPostSortedByDateDescendingAsync(int userId, [FromQuery] int skip = 0, [FromQuery] int limit = 5)
		{
			var result = await _service.GetPostsSortedByDateDescendingAsync(userId, skip, limit);
			
			return Ok(result);
		}

		[HttpGet("users/{userId}/posts/{text}", Name = "SearchPost")]
		public async Task<IActionResult> GetPostAsync(int userId, string text, [FromQuery] int skip = 0, [FromQuery] int limit = 5)
		{
			var result = await _service.GetPostsAsync(userId, text, skip, limit);
		
			return Ok(result);
		}

		[HttpDelete("users/{userId}/posts/{postId}", Name = "DeletePost")]
		public async Task<IActionResult> DeletePostAsync(int userId, string postId)
		{ 
			var result = await _service.DeletePostAsync(userId, postId);
		
			return Ok(result);
		}

		[HttpPost("posts", Name = "AddPost")]
		public async Task<IActionResult> AddPostAsync([FromForm] BasePostViewModelVisibility view)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState.GetErrorMessages());

			var result = await _service.AddPostAsync(view);
            return Ok(result);
		}

		[HttpPut("update-posts", Name = "UpdatePost")]
		public async Task<IActionResult> UpdatePostAsync([FromForm] BasePostViewModel view)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState.GetErrorMessages());

			var result = await _service.EditPostAsync(view);

			return Ok(result);
		}

		[HttpPost("posts/{postId}/like/{userId}", Name = "AddPostLike")]
		public async Task<IActionResult> AddPostLikeAsync(string postId, int userId)
		{
			var result = await _service.AddPostLikeAsync(postId, userId);

			return Ok(result);
		}

		[HttpDelete("posts/{postId}/like/{userId}", Name = "DeletePostLike")]
		public async Task<IActionResult> DeletePostLikeAsync(string postId, int userId)
		{
			var result = await _service.DeletePostLikeAsync(postId, userId);

			return Ok(result);
		}

		[HttpPost("posts/{postId}/dislike/{userId}", Name = "AddPostDislike")]
		public async Task<IActionResult> AddPostDislikeAsync(string postId, int userId)
		{
			var result = await _service.AddPostDislikeAsync(postId, userId);

			return Ok(result);
		}

		[HttpDelete("posts/{postId}/dislike/{userId}", Name = "DeletePostDislike")]
		public async Task<IActionResult> DeletePostDislikeAsync(string postId, int userId)
		{
			var result = await _service.DeletePostDislikeAsync(postId, userId);

			return Ok(result);
		}

		[HttpPost("posts/{postId}/favorite/{userId}", Name = "AddPostFavorite")]
		public async Task<IActionResult> AddPostFavoriteAsync(string postId, int userId)
		{
			var result = await _service.AddPostFavoriteAsync(postId, userId);

			return Ok(result);
		}

		[HttpDelete("posts/{postId}/favorite/{userId}", Name = "DeletePostFavorite")]
		public async Task<IActionResult> DeletePostFavoriteAsync(string postId, int userId)
		{
			var result = await _service.DeletePostFavoriteAsync(postId, userId);

			return Ok(result);
		}

		[HttpPost("posts/{postId}/share/{userId}", Name = "AddPostShare")]
		public async Task<IActionResult> AddPostShareAsync(string postId, int userId)
		{
			var result = await _service.AddPostShareAsync(postId, userId);

			return Ok(result);
		}

		[HttpDelete("posts/{postId}/share/{userId}", Name = "DeletePostShare")]
		public async Task<IActionResult> DeletePostShareAsync(string postId, int userId)
		{
			var result = await _service.DeletePostShareAsync(postId, userId);

			return Ok(result);
		}

        [HttpPut("posts/{postId}/makeitpublic/{userId}", Name = "MakeItPublic")]
        public async Task<IActionResult> MakeItPublicAsync(string postId, int userId)
        {
            var result = await _service.MakeItPublicAsync(postId, userId);

            return Ok(result);
        }

		[HttpPost("posts/{postId}/comments", Name = "AddPostComment")]
		public async Task<IActionResult> AddPostCommentAsync([FromBody] ViewModels.CommentView view, string postId)
		{
			var result = await _service.AddPostCommentAsync(view, postId);

			return Ok(result);
		}

        [HttpPut("posts/{postId}/comments", Name = "EditPostComment")]
        public async Task<IActionResult> EditPostCommentAsync([FromBody] CommentView view, string postId)
        {
            var result = await _service.EditPostCommentAsync(view, postId);

            return Ok(result);
        }

		[HttpGet("posts/{postId}/share/users/{userId}", Name = "GetSharedUsersDetailsAsync")]
		public async Task<IActionResult> GetSharedUsersDetailsAsync(string postId,  int userId)
		{
			var result = await _service.GetSharedUsersDetailsAsync(postId, userId);

			return Ok(result);
		}

        [HttpGet("posts/{postId}/like/users/{userId}", Name = "GetLikedUsersDetailsAsync")]
        public async Task<IActionResult> GetLikedUsersDetailsAsync(string postId, int userId)
        {
            var result = await _service.GetLikedUsersDetailsAsync(postId, userId);

            return Ok(result);
        }


		[HttpDelete("posts/{postId}/comments/{commentId}/{userId}", Name = "DeletePostComment")]
		public async Task<IActionResult> DeletePostCommentAsync(string postId, string commentId,int userId)
		{
			var result = await _service.DeletePostCommentAsync(postId, commentId, userId);

			return Ok(result);
		}

        [HttpPost("posts/{postId}/comments/{commentId}/report/{userId}", Name = "ReportPostComment")]
        public async Task<IActionResult> ReportPostCommentAsync([FromBody] List<ReportView> models, string postId, string commentId, int userId)
        {
            if (models == null)
                return BadRequest();

            var result = await _service.ReportPostCommentAsync(models, postId, commentId, userId);

            return Ok(result);
        }

		[HttpPut("posts/{postId}/poll-answer/{answerId}/users/{userId}", Name = "UpdatePostPollScore")]
		public async Task<IActionResult> UpdatePostPollScoreAsync(string postId, string answerId, int userId)
		{
			var result = await _service.UpdatePostPollScoreAsync(postId, answerId, userId);

			return Ok(result);
		}

		[HttpPost("posts/{postId}/reports/{userId}", Name = "AddPostReport")]
		public async Task<IActionResult> AddPostReportAsync([FromBody] List<ReportView> models, string postId, int userId)
		{
			var result = await _service.AddPostReportAsync(models, postId, userId);
            return Ok(result);
		}

		[HttpDelete("posts/{postId}/reports/{reportId}", Name = "DeletePostReport")]
		public async Task<IActionResult> DeletePostReportAsync(string postId, string reportId)
		{
			var result = await _service.DeletePostReportAsync(postId, reportId);

			return Ok(result);
		}

		[HttpGet("posts/{postId}/{skip}/{limit}/comments", Name = "GetPostComment")]
		public async Task<IActionResult> GetPostCommentAsync(string postId, int skip, int limit)
		{
			var result = await _service.GetPostCommentsAsync(postId, skip, limit);

			return Ok(result);
		}

		[HttpGet("users/{userId}/{skip}/{limit}/favorite-posts/", Name = "GetUserFavoritePosts")]
		public async Task<IActionResult> GetUserFavoritePostsAsync(int userId,int skip, int limit)
		{
			var result = await _service.GetPostsByUserFavoriteAsync(userId, skip, limit);

			return Ok(result);
		}

		[HttpPost("users/{userId}/favorite-profiles/{profileId}/", Name = "AddUserFavoriteProfile")]
		public async Task<IActionResult> AddUserFavoriteProfileAsync(int userId, int profileId)
		{
			var result = await _service.AddUserFavoriteProfileAsync(userId, profileId);
	
			return Ok(result);
		}

		[HttpDelete("users/{userId}/favorite-profiles/{profileId}/", Name = "DeleteUserFavoriteProfile")]
		public async Task<IActionResult> DeleteUserFavoriteProfileAsync(int userId, int profileId)
		{
			var result = await _service.DeleteUserFavoriteProfileAsync(userId, profileId);
		
			return Ok(result);
		}

		[HttpDelete("users/{userId}/viewers/{viewerId}/", Name = "AddUserProfileViewer")]
		public async Task<IActionResult> AddUserProfileViewerAsync(int userId, int profileId)
		{
			var result = await _service.AddUserProfileViewerAsync(userId, profileId);
		
			return Ok(result);
		}

		[HttpGet("users/{userId}/notifications", Name = "GetNotifications")]
		public async Task<IActionResult> GetNotificationsAsync(int userId)
		{
			var result = await _service.GetNotificationObjectAsync(userId);
			
			return Ok(result);
		}

		[HttpGet("users/{userId}/unseen-notifications-count", Name = "GetNotificationsUnseenCount")]
		public async Task<IActionResult> GetNotificationsUnseenCount(int userId)
		{
			var result = await _service.GetNotificationUnseenCountObjectAsync(userId);

			return Ok(result);
		}

		[HttpPut("users/{userId}/notifications/{notificationId}/as-read", Name = "UpdateNotificationAsRead")]
		public async Task<IActionResult> UpdateNotificationAsReadAsync(int userId, string notificationId)
		{
			var result = await _service.UpdateNotificationAsReadAsync(userId, notificationId);
		
			return Ok(result);
		}

        [HttpPut("users/{userId}/notifications/unseen-count", Name = "UpdateUnseenNotification")]
        public async Task<IActionResult> UpdateNotificationAsReadAsync(int userId)
        {
            var result = await _service.UpdateUnseenNotificationAsync(userId);

            return Ok(result);
        }

		[HttpPost("users/{userId}/following/{followingId}", Name = "AddUserFollowing")]
		public async Task<IActionResult> AddUserFollowingAsync(int userId, int followingId)
		{
			var result = await _service.AddUserFollowingAsync(userId, followingId);
			if (!result.Success)
				return BadRequest(result);

			return Ok(result);
		}

		[HttpDelete("users/{userId}/following/{followingId}", Name = "DeleteUserFollowing")]
		public async Task<IActionResult> DeleteUserFollowingAsync(int userId, int followingId)
		{
			var result = await _service.DeleteUserFollowingAsync(userId, followingId);
	
			return Ok(result);
		}

		[HttpPost("users/{userId}/follower/{followerId}", Name = "AddUserFollower")]
		public async Task<IActionResult> AddUserFollowerAsync(int userId, int followerId)
		{
			var result = await _service.AddUserFollowerAsync(userId, followerId);
		
			return Ok(result);
		}

		[HttpDelete("users/{userId}/follower/{followerId}", Name = "DeleteUserFollower")]
		public async Task<IActionResult> DeleteUserFollowerAsync(int userId, int followerId)
		{
			var result = await _service.DeleteUserFollowerAsync(userId, followerId);
		
			return Ok(result);
		}

		[HttpPost("upload-file", Name = "UploadFile")]
		public async Task<IActionResult> UploadFileAsync(IFormFile file)
		{
			var result = await _service.UploadFile(file.FileName, file.OpenReadStream());
		
			return Ok(result);
		}


        [HttpGet("activity-logs/{userId}/{take}/{page}", Name = "GetActivityLogs")]
        public async Task<IActionResult>GetActivityLogsAsync(int userId, int take, int page)
        {
            var result = await _service.GetActivityLogsAsync(userId, take, page);
            return Ok(result);
        }

        [HttpGet("get-announcements", Name = "GetAnnouncementsAsync")]
        public async Task<IActionResult> GetAnnouncementsAsync()
        {
            var result = await _service.GetAnnouncementsAsync();
            return Ok(result);
        }

		[HttpGet("get-survey-details/{surveyId}/{userId}", Name = "GetSurveyQuestionsAsync")]
		public async Task<IActionResult> GetSurveyQuestionsAsync(int surveyId,int userId)
		{
			var result = await _service.GetSurveyQuestionsAsync(surveyId, userId);
			return Ok(result);
		}

		[HttpPost("post/survey/answers", Name = "AddUserSurveyAnswersAsync")]
		public async Task<IActionResult> AddUserSurveyAnswersAsync([FromForm]SurveyAnswersView model)
		{
			var result = await _service.AddUserSurveyAnswersAsync(model);
		
			return Ok(result);
		}



	}
}