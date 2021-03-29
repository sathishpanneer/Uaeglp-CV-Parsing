using System;
using System.Collections.Generic;
using System.Linq;
using Uaeglp.MongoModels;
using Uaeglp.ViewModels;
using MongoDB.Bson;
using Uaeglp.Models;

namespace Uaeglp.Services.Extensions
{
	public static class Converter
	{
		//public static Post ToPost(this PostView postView)
		//{
		//	var post = new Post
		//	{
		//		UserID = postView.UserID,
		//		InteractionCounter = (postView.InteractionCounter == null) ? postView.InteractionCounter : 0
		//	};

		//	post.TypeID = postView?.TypeID;

		//	if (postView.Text != null)
		//		post.Text = postView.Text;

		//	if (postView.YoutubeVideoID != null)
		//		post.YoutubeVideoID = postView.YoutubeVideoID;

		//	if (postView.Category != null)
		//		post.Category = postView.Category;

		//	if (postView.Author != null)
		//		post.Author = postView.Author;

		//	if (postView.IsAdminCreated != null)
		//		post.IsAdminCreated = postView.IsAdminCreated;

		//	if (postView.Created != null)
		//		post.Created = postView.Created.Value.ToUniversalTime();

		//	if (postView.Created != null)
		//		post.Modified = postView.Modified.Value.ToUniversalTime();

		//	if (postView.CreatedBy != null)
		//		post.CreatedBy = postView.CreatedBy;

		//	if (postView.ModifiedBy != null)
		//		post.ModifiedBy = postView.ModifiedBy;

		//	if (postView.Id != null)
		//		post.Id = new BsonObjectId(new ObjectId(postView.Id));
		//	else
		//		post.Id = ObjectId.GenerateNewId();

		//	if (postView.PostSharedID != null)
		//		post.PostSharedID = new BsonObjectId(new ObjectId(postView.PostSharedID));

		//	if (postView.ImageIDs != null)
		//	{
		//		post.ImageIDs = new BsonArray();
		//		foreach (var id in postView.ImageIDs)
		//		{
		//			post.ImageIDs.Add(id);
		//		}
		//	}
		//	if (postView.Likes != null)
		//	{
		//		post.Likes = new BsonArray();
		//		foreach (var like in postView.Likes)
		//		{
		//			post.Likes.Add(like);
		//		}
		//	}
		//	if (postView.Dislikes != null)
		//	{
		//		post.Dislikes = new BsonArray();
		//		foreach (var dislike in postView.Dislikes)
		//		{
		//			post.Dislikes.Add(dislike);
		//		}
		//	}
		//	if (postView.Favorites != null)
		//	{
		//		post.Favorites = new BsonArray();
		//		foreach (var favorite in postView.Favorites)
		//		{
		//			post.Favorites.Add(favorite);
		//		}
		//	}
		//	if (postView.Shares != null)
		//	{
		//		post.Shares = new BsonArray();
		//		foreach (var share in postView.Shares)
		//		{
		//			post.Shares.Add(share);
		//		}
		//	}
		//	post.Comments = postView.Comments?.ToList().Select(x => x.ToComment()).ToList();
		//	post.Reports = postView.Reports?.ToList().Select(x => x.ToReport()).ToList();
		//	post.SurveyPost = postView.Survey?.ToSurvey();
		//	post.PollPost = postView.Poll?.ToPoll();
		//	return post;
		//}

		//public static PostView ToPostView(this Post post)
		//{
		//	var view = new PostView();

		//	view.Id = post.Id.Value.ToString();
		//	view.UserID = post.UserID?.Value;
		//	view.Text = post.Text?.Value;
		//	view.TypeID = post.TypeID?.Value;
		//	view.YoutubeVideoID = post.YoutubeVideoID?.Value;
		//	view.InteractionCounter = post.InteractionCounter?.Value;
		//	view.PostSharedID = post.PostSharedID?.Value.ToString();
		//	view.Category = post.Category?.Value;
		//	view.Author = post.Author?.Value;
		//	view.IsAdminCreated = post.IsAdminCreated?.Value;
		//	view.Created = post.Created?.ToUniversalTime();
		//	view.Modified = post.Modified?.ToUniversalTime();
		//	view.CreatedBy = post.CreatedBy?.Value;
		//	view.ModifiedBy = post.ModifiedBy?.Value;
		//	view.ImageIDs = post.ImageIDs?.ToList().Select(x => x.ToString()).ToList();
		//	view.FileIDs = post.FileIDs?.ToList().Select(x => x.ToString()).ToList();
		//	view.Likes = post.Likes?.ToList().Select(x => x.ToInt32()).ToList();
		//	view.Dislikes = post.Dislikes?.ToList().Select(x => x.ToInt32()).ToList();
		//	view.Shares = post.Shares?.ToList().Select(x => x.ToInt32()).ToList();
		//	view.Favorites = post.Favorites?.ToList().Select(x => x.ToInt32()).ToList();
		//	view.Comments = post.Comments?.ToList().Select(x => x.ToCommentView()).ToList();
		//	view.Reports = post.Reports?.ToList().Select(x => x.ToReportView()).ToList();
		//	view.Survey = post.SurveyPost?.ToSurveyView();
		//	view.Poll = post.PollPost?.ToPollView();
		//	view.LikeCount = post.Likes?.Count ?? 0;
		//	view.ShareCount = post.Shares?.Count ?? 0;
		//	view.CommentCount = post.Comments?.Count ?? 0;
		//	return view;
		//}

		//public static IList<PostView> ToPostViewCollection(this IList<Post> posts)
		//{
		//	if (posts == null)
		//		return new List<PostView>();

		//	var postViews = new List<PostView>();

		//	foreach (var item in posts)
		//	{
		//		postViews.Add(item.ToPostView());
		//	}
		//	return postViews;
		//}

		//public static CommentView ToCommentView(this Comment comment)
		//{
		//	return new CommentView
		//	{
		//		Id = comment.Id?.ToString(),
		//		UserID = Convert.ToInt32(comment.UserID?.Value),
		//		Text = comment.Text?.Value,
		//		IsAdminCreated = Convert.ToBoolean(comment.IsAdminCreated?.Value),
		//		IsDeleted = Convert.ToBoolean(comment.IsDeleted?.Value),
		//		Created = Convert.ToDateTime(comment.Created),
		//		Modified = Convert.ToDateTime(comment.Modified)
		//	};
		//}

		//public static RoomView ToRoomView(this Room room)
		//{
		//	return new RoomView
		//	{
		//		ID = room.ID.ToString(),
		//		RoomTitle = room.RoomTitle.Value,
		//		OwnerID = room.OwnerID.Value,
		//		RoomTypeID = room.RoomTypeID.Value,
		//		NumberOfMembers = room.NumberOfMembers.Value,
		//		CreatedOn = room.CreatedOn.ToUniversalTime(),
		//		LastModifiedOn = room.LastModifiedOn.ToUniversalTime(),
		//		MembersIDs = room.MembersIDs?.ToList().Select(x => x.ToInt32()).ToList(),
		//		ArchivedMembersIDs = room.ArchivedMembersIDs?.ToList().Select(x => x.ToInt32()).ToList(),
		//		//Messages = room.Messages?.ToList().Select(x => x.ToMessageView()).ToList(),
		//		//UnreadMessages = room.UnreadMessages?.ToList().Select(x => x.ToUnreadMessageViewView()).ToList(),
		//		ModifiedBy = room.ModifiedBy.ToString(),
		//	};
		//}

		//public static IList<RoomView> ToRoomViewCollection(this IList<Room> rooms)
		//{
		//	var roomViews = new List<RoomView>();
		//	foreach (var item in rooms)
		//	{
		//		roomViews.Add(item.ToRoomView());
		//	}
		//	return roomViews;
		//}

		//public static Room ToRoom(this RoomView roomView)
		//{
		//	var room = new Room
		//	{
		//		RoomTitle = roomView.RoomTitle,
		//		OwnerID = roomView.OwnerID,
		//		RoomTypeID = roomView.RoomTypeID,
		//		NumberOfMembers = roomView.NumberOfMembers,
		//		CreatedOn = roomView.CreatedOn.ToUniversalTime(),
		//		LastModifiedOn = roomView.LastModifiedOn.ToUniversalTime(),
		//		MembersIDs = new BsonArray(roomView.MembersIDs?.ToList()),
		//		ArchivedMembersIDs = new BsonArray(roomView.ArchivedMembersIDs?.ToList()),
		//		//Messages = roomView.Messages?.ToList().Select(x => x.ToMessageView()).ToList(),
		//		//UnreadMessages = roomView.UnreadMessages?.ToList().Select(x => x.ToUnreadMessageViewView()).ToList(),
		//		ModifiedBy = roomView.ModifiedBy.ToString(),
		//	};

		//	if (roomView.ID != null)
		//		room.ID = new ObjectId(roomView.ID);
		//	else
		//		room.ID = ObjectId.GenerateNewId();
		//	return room;
		//}

		//public static Message ToMessage(this MessageView messageView)
		//{
		//	var Message = new Message
		//	{
		//		MessageText = messageView.MessageText,
		//		TypeID = messageView.TypeID,
		//	};
		//	if (messageView.ID != null)
		//		Message.ID = new ObjectId(messageView.ID);
		//	else
		//		Message.ID = ObjectId.GenerateNewId();

		//	if (messageView.FilesIDs != null)
		//	{
		//		Message.FilesIDs = new BsonArray();
		//		foreach (var item in messageView.FilesIDs)
		//		{
		//			Message.FilesIDs.Add(item);
		//		}
		//	}
		//	if (messageView.ImagesIDs != null)
		//	{
		//		Message.ImagesIDs = new BsonArray();
		//		foreach (var item in messageView.ImagesIDs)
		//		{
		//			Message.ImagesIDs.Add(item);
		//		}
		//	}
		//	if (messageView.SeenByIDs != null)
		//	{
		//		Message.SeenByIDs = new BsonArray();
		//		foreach (var item in messageView.FilesIDs)
		//		{
		//			Message.SeenByIDs.Add(item);
		//		}
		//	}
		//	return Message;
		//}

		//public static MessageView ToMessageView(this Message Message)
		//{
		//	return new MessageView
		//	{
		//		ID = Message.ID.Value.ToString(),
		//		MessageText = Message.MessageText.Value,
		//		TypeID = Message.TypeID.Value,
		//		FilesIDs = Message.FilesIDs?.ToList().Select(x => x.ToString()).ToList(),
		//		ImagesIDs = Message.ImagesIDs?.ToList().Select(x => x.ToString()).ToList(),
		//		SeenByIDs = Message.SeenByIDs?.ToList().Select(x => Convert.ToInt32(x)).ToList()
		//	};
		//}

		//public static UserView ToUserView(this MongoModels.User user)
		//{
		//	return new UserView
		//	{
		//		Id = user.Id.Value,
		//		MyFavouriteProfilesIDs = user.MyFavouriteProfilesIDs?.ToList().Select(x => x.ToInt32()).ToList() ?? new List<int>(),
		//		FollowersIDs = user.FollowersIDs?.ToList().Select(x => x.ToInt32()).ToList() ?? new List<int>(),
		//		FollowingIDs = user.FollowingIDs?.ToList().Select(x => x.ToInt32()).ToList() ?? new List<int>()
		//	};
		//}

		//public static UserView ToUserView(this Models.User user)
		//{
		//	return new UserView
		//	{

		//	};
		//}

		//public static ReportView ToReportView(this Report report)
		//{
		//	return new ReportView
		//	{
		//		ID = report.ID?.ToString(),
		//		UserID = Convert.ToInt32(report.UserID?.Value),
		//		ReasonTypeID = Convert.ToInt32(report.ReasonTypeID?.Value),
		//		Reason = report.Reason?.Value,
		//		Created = report.Created.ToUniversalTime()
		//	};
		//}

		//public static Report ToReport(this ReportView reportView)
		//{
		//	var report = new Report
		//	{
		//		UserID = reportView.UserID,
		//		ReasonTypeID = reportView.ReasonTypeID,
		//		Reason = reportView.Reason,
		//		Created = reportView.Created
		//	};
		//	if (reportView.ID != null)
		//		report.ID = new ObjectId(reportView.ID);
		//	else
		//		report.ID = ObjectId.GenerateNewId();
		//	return report;
		//}

		//public static ViewerView ToViewerView(this Viewer viewer)
		//{
		//	return new ViewerView
		//	{
		//		ViewerID = viewer.ViewerID.Value,
		//		Date = viewer.Date.ToUniversalTime()
		//	};
		//}

		//public static Viewer ToViewer(this ViewerView viewerView)
		//{
		//	return new Viewer
		//	{
		//		ViewerID = viewerView.ViewerID,
		//		Date = viewerView.Date
		//	};
		//}

		//public static Comment ToComment(this CommentView commentView)
		//{
		//	var comment = new Comment
		//	{
		//		UserID = commentView.UserID,
		//		Text = commentView.Text,
		//		IsAdminCreated = commentView.IsAdminCreated,
		//		IsDeleted = commentView.IsDeleted,
		//		Created = commentView.Created,
		//		Modified = commentView.Modified
		//	};
		//	if (commentView.Id != null)
		//		comment.Id = new ObjectId(commentView.Id);
		//	else
		//		comment.Id = ObjectId.GenerateNewId();
		//	return comment;
		//}

		//public static MongoModels.Survey ToSurvey(this SurveyView surveyView)
		//{
		//	var survey = new MongoModels.Survey
		//	{
		//		SurveyID = surveyView.SurveyID,
		//		Text = surveyView.Text,
		//		LinkURLDescription = surveyView.LinkURLDescription,
		//		RedirectionURL = surveyView.RedirectionURL
		//	};
		//	if (surveyView.ID != null)
		//		survey.Id = new ObjectId(surveyView.ID);
		//	else
		//		survey.Id = ObjectId.GenerateNewId();
		//	return survey;
		//}

		//public static SurveyView ToSurveyView(this MongoModels.Survey survey)
		//{
		//	return new SurveyView
		//	{
		//		ID = survey.Id?.ToString(),
		//		SurveyID = Convert.ToInt32(survey.SurveyID?.ToString()),
		//		Text = survey.Text?.ToString(),
		//		LinkURLDescription = survey.LinkURLDescription?.ToString(),
		//		RedirectionURL = survey.RedirectionURL?.ToString()
		//	};
		//}

		//public static PollPost ToPoll(this PollView pollView)
		//{
		//	var poll = new PollPost
		//	{
		//		Question = pollView.Question,
		//		Answers = pollView.Answers?.ToList().Select(x => x.ToPollAnswer()).ToList()
		//	};
		//	if (pollView.ID != null)
		//		poll.Id = new ObjectId(pollView.ID);
		//	else
		//		poll.Id = ObjectId.GenerateNewId();
		//	return poll;
		//}

		//public static PollView ToPollView(this PollPost poll)
		//{
		//	return new PollView
		//	{
		//		ID = poll.Id?.ToString(),
		//		Question = poll.Question?.ToString(),
		//		Answers = poll.Answers?.ToList().Select(x => x.ToPollAnswerView()).ToList()
		//	};
		//}

		//public static PollAnswer ToPollAnswer(this PollAnswerView answerView)
		//{
		//	var answer = new PollAnswer
		//	{
		//		Answer = answerView.Answer,
		//		Score = answerView.Score
		//	};
		//	if (answerView.ID != null)
		//		answer.Id = new ObjectId(answerView.ID);
		//	else
		//		answer.Id = ObjectId.GenerateNewId();

		//	if (answerView.Users != null)
		//	{
		//		answer.Users = new BsonArray();
		//		foreach (var item in answerView.Users)
		//		{
		//			answer.Users.Add(item);
		//		}
		//	}
		//	return answer;
		//}

		//public static PollAnswerView ToPollAnswerView(this PollAnswer answer)
		//{
		//	return new PollAnswerView
		//	{
		//		ID = answer.Id?.ToString(),
		//		Answer = answer.Answer?.ToString(),
		//		Score = Convert.ToInt32(answer.Score),
		//		Users = answer.Users?.ToList().Select(x => x.ToInt32()).ToList()
		//	};
		//}

		//public static ProfileView ToProfileView(this Profile profile)
		//{
		//	var view = new ProfileView
		//	{
		//		Id = profile.Id,
		//		FirstName = profile.FirstNameEn
		//	};
		//	return view;
		//}

		//public static IList<NotificationView> ToNotificationViewCollection(this IList<Notification> notifications)
		//{
		//	var notificationViews = new List<NotificationView>();

		//	foreach (var item in notifications)
		//	{
		//		notificationViews.Add(item.ToNotificationView());
		//	}
		//	return notificationViews;
		//}

		//public static NotificationView ToNotificationView(this Notification notification)
		//{
		//	var notificationView = new NotificationView
		//	{
		//		Id = notification.Id.Value.ToString(),
		//		ParentID = notification.ParentID.Value,
		//		ParentTypeID = notification.ParentTypeID.Value,
		//		ActionID = notification.ActionID.Value,
		//		SenderID = notification.SenderID.Value,
		//		IsRead = notification.IsRead.Value,
		//		GeneralNotification = notification.GeneralNotification.Value,
		//		Created = notification.Created.ToUniversalTime(),
		//		Modified = notification.Modified.ToUniversalTime(),
		//	};
		//	return notificationView;
		//}
	}
}
