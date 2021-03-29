using System.Collections.Generic;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ProfileViewModels;
using Uaeglp.ViewModels.SurveyViewModels;

namespace Uaeglp.Contract.Communication
{
    public interface ISocialResponse : IBaseResponse
    {
        PostView Post { get; set; }
        UserView User { get; set; }
        RoomView Room { get; set; }
        SurveyQuestionViewModel SurveyQuestionView { get; set; }
        IList<RoomView> Rooms { get; set; }
        IList<PostView> Posts { get; set; }
        IList<AllPostView> AllPosts { get; set; }
        IList<CommentView> Comments { get; set; }
        List<NotificationView> ActivityLogs { get; set; }
        List<AnnouncementView> Announcements { get; set; }
        List<SurveyDetail> SurveyDetail { get; set; }
        NotificationGenericObjectView NotificationGenericObject { get; set; }
        int UnseenNotificationCount { get; set; }
        IList<string> FileIds { get; set; }
        byte[] File { get; set; }

        long TotalPostCount { get; set; }
        long TotalActivityLogCount { get; set; }

        IList<FilterTypeViewModel> FilterType { get; set; }

        List<PublicProfileView> ShareUserList { get; set; }

        bool IsSurveyAlreadySubmitted { get; set; }
    }
}
