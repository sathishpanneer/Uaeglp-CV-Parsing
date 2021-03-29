using System;
using System.Collections.Generic;
using System.Net;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ProfileViewModels;
using Uaeglp.ViewModels.SurveyViewModels;

namespace Uaeglp.Services.Communication
{
	public class SocialResponse : BaseResponse, ISocialResponse
	{
		public PostView Post { get; set; }
		public UserView User { get; set; }
		public RoomView Room { get; set; }
		public SurveyQuestionViewModel SurveyQuestionView { get; set; }
		public IList<RoomView> Rooms { get; set; }
		public IList<PostView> Posts { get; set; }
		public IList<AllPostView> AllPosts { get; set; }
		public IList<CommentView> Comments { get; set; }
		public List<NotificationView> ActivityLogs { get; set; }
		public List<AnnouncementView> Announcements { get; set; }
		public List<SurveyDetail> SurveyDetail { get; set; }
		public NotificationGenericObjectView NotificationGenericObject { get; set; }
		public int UnseenNotificationCount { get; set; }
		public IList<string> FileIds { get; set; }
		public List<PublicProfileView> ShareUserList { get; set; }
		public byte[] File { get; set; }

		public bool IsSurveyAlreadySubmitted { get; set; }
        public long TotalPostCount { get; set; }
        public long TotalActivityLogCount { get; set; }
		public long TotalFavoriteCount { get; set; }
		public long TotalCommentCount { get; set; }
		public long SearchPostCout { get; set; }

		public IList<FilterTypeViewModel> FilterType { get; set; }

		private SocialResponse(bool success, string message, UserView user) : base(success, message)
		{
			User = user;
		}

        private SocialResponse(bool success, string message, bool isSurveyAlreadySubmitted) : base(success, message)
        {
            IsSurveyAlreadySubmitted = isSurveyAlreadySubmitted;
        }

		private SocialResponse(bool success, string message, List<PublicProfileView> user) : base(success, message)
        {
            ShareUserList = user;
        }

		private SocialResponse(bool success, string message, SurveyQuestionViewModel user) : base(success, message)
        {
            SurveyQuestionView = user;
        }
		private SocialResponse(bool success, string message, PostView post) : base(success, message)
		{
			Post = post;
		}
		private SocialResponse(bool success, string message, RoomView room) : base(success, message)
		{
			Room = room;
		}
		private SocialResponse(bool success, string message, IList<RoomView> rooms) : base(success, message)
		{
			Rooms = rooms;
		}
		private SocialResponse(bool success, string message, IList<PostView> posts) : base(success, message)
		{
			Posts = posts;
		}
		private SocialResponse(bool success, string message, IList<AllPostView> allposts) : base(success, message)
		{
			AllPosts = allposts;
		}
		private SocialResponse(bool success, string message, IList<CommentView> comments) : base(success, message)
		{
			Comments = comments;
		}

        private SocialResponse(bool success, string message, List<NotificationView> activityLogs) : base(success, message)
        {
            ActivityLogs = activityLogs;
        }

        private SocialResponse(bool success, string message, List<AnnouncementView> announcements) : base(success, message)
        {
            Announcements = announcements;
        }
		private SocialResponse(bool success, string message, List<SurveyDetail> surveyDetails) : base(success, message)
		{
			SurveyDetail = surveyDetails;
		}
		private SocialResponse(bool success, string message, NotificationGenericObjectView view) : base(success, message)
		{
			NotificationGenericObject = view;
		}

		private SocialResponse(bool success, string message, int view) : base(success, message)
		{
			UnseenNotificationCount = view;
		}

		private SocialResponse(bool success, string message, IList<string> fileIds) : base(success, message)
		{
			FileIds = fileIds;
		}

		private SocialResponse(bool success, string message, byte[] file) : base(success, message)
		{
			File = file;
		}
		private SocialResponse(bool success, string message, IList<FilterTypeViewModel> filterTypes) : base(success, message)
		{
			FilterType = filterTypes;
		}

		public SocialResponse(UserView user) : this(true, string.Empty, user)
		{ }

        public SocialResponse(SurveyQuestionViewModel user) : this(true, string.Empty, user)
        { }
		public SocialResponse(PostView post) : this(true, string.Empty, post)
		{ }

		public SocialResponse(IList<PostView> posts) : this(true, string.Empty, posts)
		{ }
		public SocialResponse(IList<AllPostView> allposts) : this(true, string.Empty, allposts)
		{ }
		public SocialResponse(IList<CommentView> comments) : this(true, string.Empty, comments)
		{ }
        public SocialResponse(List<NotificationView> activityLog) : this(true, string.Empty, activityLog)
        { }

        public SocialResponse(List<AnnouncementView> announcement) : this(true, string.Empty, announcement)
        { }
		public SocialResponse(List<SurveyDetail> surveyDetails) : this(true, string.Empty, surveyDetails)
		{ }

        public SocialResponse(List<PublicProfileView> profileViews) : this(true, string.Empty, profileViews)
        { }
		public SocialResponse(IList<RoomView> rooms) : this(true, string.Empty, rooms)
		{ }
		public SocialResponse(RoomView room) : this(true, string.Empty, room)
		{ }
		public SocialResponse(bool isSurveyAlreadySubmitted) : this(true, string.Empty, isSurveyAlreadySubmitted)
        { }
		public SocialResponse(NotificationGenericObjectView view) : this(true, string.Empty, view)
		{ }

		public SocialResponse(int view) : this(true, string.Empty, view)
		{ }
		public SocialResponse(byte[] file) : this(true, string.Empty, file)
		{ }
		public SocialResponse(IList<string> fileIds) : this(true, string.Empty, fileIds)
		{ }
		public SocialResponse(IList<FilterTypeViewModel> filterTypes) : this(true, string.Empty, filterTypes)
		{ }
		public SocialResponse(string message, HttpStatusCode status) : base(false, message, status)
		{ }
		public SocialResponse(Exception e) : base(e)
		{ }
		public SocialResponse() : base()
		{ }
	}
}
