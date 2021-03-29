using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Uaeglp.Contract.Communication;
using Uaeglp.MongoModels;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.PostViewModels;
using Uaeglp.ViewModels.SurveyViewModels;

namespace Uaeglp.Contracts
{
	public interface ISocialService
	{
		Task<ISocialResponse> AddPostAsync(BasePostViewModelVisibility model);
		Task<ISocialResponse> EditPostAsync(BasePostViewModel model);
		Task<ISocialResponse> DeletePostAsync(int userId, string postId);
		Task<ISocialResponse> GetPostsAsync(GetPostsViewModel postsViewModel);
        Task<ISocialResponse> GetPostAsync(string postId, int userId);
		Task<ISocialResponse> GetFilterTypeAsync(int userId);
		Task<ISocialResponse> GetMyPostsAsync(int userId, int skip = 0, int limit = 5);
		Task<ISocialResponse> GetPostsByAdminCreatedAsync(int userId, int skip = 0, int limit = 5);
		Task<ISocialResponse> GetPostsByGroupCreatedAsync(int userId, int skip = 0, int limit = 5);
		Task<ISocialResponse> GetPostsSortedByDateAscendingAsync(int userId, int skip = 0, int limit = 5);
		Task<ISocialResponse> GetPostsSortedByDateDescendingAsync(int userId, int skip = 0, int limit = 5);
		Task<ISocialResponse> GetPostsAsync(int userId, string text, int skip = 0, int limit = 5);
		Task<ISocialResponse> GetPostsByUserFavoriteAsync(int userId, int skip = 0, int limit = 5);
		Task<ISocialResponse> AddPostLikeAsync(string postId, int userId);
		Task<ISocialResponse> DeletePostLikeAsync(string postId, int userId);
		Task<ISocialResponse> AddPostDislikeAsync(string postId, int userId);
		Task<ISocialResponse> DeletePostDislikeAsync(string postId, int userId);
		Task<ISocialResponse> AddPostFavoriteAsync(string postId, int userId);
		Task<ISocialResponse> DeletePostFavoriteAsync(string postId, int userId);
		Task<ISocialResponse> AddPostShareAsync(string postId, int userId);
		Task<ISocialResponse> DeletePostShareAsync(string postId, int userId);
		Task<ISocialResponse> AddPostCommentAsync(CommentView view, string postId);
        Task<ISocialResponse> EditPostCommentAsync(CommentView view, string postId);
		Task<ISocialResponse> DeletePostCommentAsync(string postId, string commentId,  int userId);
		Task<ISocialResponse> ReportPostCommentAsync(List<ReportView> models, string postId, string commentId, int userId);

        Task<ISocialResponse> GetSharedUsersDetailsAsync(string postId, int userId);
        Task<ISocialResponse> GetLikedUsersDetailsAsync(string postId, int userId);
		Task<ISocialResponse> MakeItPublicAsync(string postId ,int userId);
        Task<ISocialResponse> UpdatePostPollScoreAsync(string postId, string answerId, int userId);
		Task<ISocialResponse> GetPostCommentsAsync(string postId, int skip, int limit);
		Task<ISocialResponse> AddPostReportAsync(List<ReportView> models, string postId, int userId);
		Task<ISocialResponse> DeletePostReportAsync(string reportId, string postId);
		Task<ISocialResponse> AddUserFollowerAsync(int userId, int followerId);
		Task<ISocialResponse> AddUserFollowingAsync(int userId, int followingId);
		Task<ISocialResponse> AddUserFavoriteProfileAsync(int userId, int profileId);
		Task<ISocialResponse> DeleteUserFollowerAsync(int userId, int followerId);
		Task<ISocialResponse> DeleteUserFollowingAsync(int userId, int followingId);
		Task<ISocialResponse> DeleteUserFavoriteProfileAsync(int userId, int profileId);
		Task<ISocialResponse> AddUserProfileViewerAsync(int userId, int viewerId);
		Task<ISocialResponse> GetNotificationObjectAsync(int userId);
		Task<ISocialResponse> GetNotificationUnseenCountObjectAsync(int userId);
		Task<ISocialResponse> AddNotificationAsync(int userId, int typeId, ActionType actionId, string parentPostId,
            ParentType parentTypeId, int senderUserId);
		Task<ISocialResponse> UpdateNotificationAsReadAsync(int userId, string notificationId);

        Task<ISocialResponse> UpdateUnseenNotificationAsync(int userId);
		Task<ISocialResponse> UploadFile(string filename, Stream file);
		Task<ISocialResponse> GetFile(string fileId);
		Task<ISocialResponse> GetUserFollowersAndFollowingAsync(int userId);
		Task<ISocialResponse> AddRoomAsync(RoomView view);
		Task<ISocialResponse> DeleteRoomAsync(string roomId);
		Task<ISocialResponse> GetRoomAsync(int userId);
		Task<ISocialResponse> GetRoomAsync(string roomId);
		Task<ISocialResponse> AddRoomMemberAsync(string roomId, int userId);
		Task<ISocialResponse> DeleteRoomMemberAsync(string roomId, int userId);
		Task<ISocialResponse> AddRoomArchiveMemberAsync(string roomId, int memberId);
		Task<ISocialResponse> DeleteRoomArchiveMemberAsync(string roomId, int memberId);
		Task<ISocialResponse> AddRoomMessageAsync(string roomId, MessageView message);
		Task<ISocialResponse> DeleteRoomMessageAsync(string roomId, string messageId);
        Task<ISocialResponse> GetActivityLogsAsync(int userId, int take, int page);
        Task<ISocialResponse> GetAnnouncementsAsync();
        Task<ISocialResponse> AddUserSurveyAnswersAsync(SurveyAnswersView model);
        Task<ISocialResponse> GetSurveyQuestionsAsync(int surveyId, int userId);
		Task<ISocialResponse> SearchPostsAsync(int userId,string text = "", int skip = 0, int limit = 5);

	}
}
