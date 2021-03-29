using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Uaeglp.Utilities;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.ViewModels
{
	public class PostView
    {
        private List<CommentView> _commentViews = new List<CommentView>();
		public string ID { get; set; }
		public int UserID { get; set; }
		public IList<string> ImageIDs { get; set; } = new List<string>();
		public IList<string> FileIDs { get; set; } = new List<string>();
		public IList<int> Likes { get; set; } = new List<int>();
		public IList<int> Favorites { get; set; } = new List<int>();
		public IList<int> Dislikes { get; set; } = new List<int>();
		public IList<int> Shares { get; set; }

        public List<CommentView> Comments
        {
            get { return _commentViews?.OrderBy(k => k.Created).ToList(); }
            set => _commentViews = value;
        }
        
        public List<ReportView> Reports { get; set; }
		public PostType TypeID { get; set; }
		public string Text { get; set; }
		public string YoutubeVideoID { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
		public DateTime Modified { get; set; } = DateTime.Now;
		public string ModifiedBy { get; set; }
		public string CreatedBy { get; set; }
		public int InteractionCounter { get; set; }
		public string PostSharedID { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsAdminCreated { get; set; }
		public bool IsPublic { get; set; }
        public bool IsPinned { get; set; }
        public int PinOrder { get; set; }
        public string Category { get; set; }
		public string Author { get; set; }
		public SurveyView Survey { get; set; } = new SurveyView();
		public PollView Poll { get; set; } = new PollView();
        public string UserName { get; set; }
        public string ProfileImageUrl => ConstantUrlPath.ProfileImagePath + UserImageFileId;
        public int LikeCount => Likes.Count;
        public int ShareCount => Shares.Count;
        public int CommentCount => Comments.Count;
        public int? GroupID { get; set; }
        public string GroupNameEN { get; set; }
        public string GroupNameAR { get; set; }
        public string NameEn { get; set; }
		public string NameAr { get; set; }
		public string TitleEn { get; set; }
		public string TitleAr { get; set; }
		public string DocumentName { get; set; }
		public int UserImageFileId { get; set; }

		public bool Liked { get; set; }
		public bool IsAmCommented { get; set; }
        public bool Favorited { get; set; }
        public bool IsAmFollowing { get; set; }
        public bool IsAnsweredThisPoll { get; set; }
        public bool IsAdminPostShared { get; set; }

        public PublicProfileView SharedUserDetails { get; set; }

        public string ImageFileUrl
        {
            get
            {
                if (!ImageIDs.Any())
                {
                    return "";
                }

                return ConstantUrlPath.PostFileDownloadPath + ImageIDs.FirstOrDefault();
            }
        }

        public string DocumentFileUrl
        {
            get
            {
                if (!FileIDs.Any())
                {
                    return "";
                }

                return ConstantUrlPath.PostFileDownloadPath + FileIDs.FirstOrDefault();
            }
        }

        public string YouTubeVideoUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(YoutubeVideoID))
                {
                    return "";
                }

                return Guid.TryParse(YoutubeVideoID, out var guid) ? "" : ConstantUrlPath.YouTubeUrlPath + YoutubeVideoID;
            }
        }

        //public string YouTubeVideoUrl { get; set; } = "";

        public string PostVideoUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(YoutubeVideoID))
                {
                    return "";
                }

                return Guid.TryParse(YoutubeVideoID,out var guid) ? ConstantUrlPath.PostVideoUrlPath + YoutubeVideoID : "";
            }
        }

        //public string PostVideoUrl { get; set; } = "";


    }

    public class AllPostView
    {
        private List<CommentView> _commentViews = new List<CommentView>();
        public string ID { get; set; }
        public int UserID { get; set; }
        public IList<string> ImageIDs { get; set; } = new List<string>();
        public IList<string> FileIDs { get; set; } = new List<string>();
        public IList<int> Likes { get; set; } = new List<int>();
        public IList<int> Favorites { get; set; } = new List<int>();
        public IList<int> Dislikes { get; set; } = new List<int>();
        public IList<int> Shares { get; set; }
        public List<CommentView> Comments
        {
            get { return _commentViews?.OrderBy(k => k.Created).Take(10).ToList(); }
            set => _commentViews = value;
        }
        public List<ReportView> Reports { get; set; }
        public PostType TypeID { get; set; }
        public string Text { get; set; }
        public string YoutubeVideoID { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Modified { get; set; } = DateTime.Now;
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public int InteractionCounter { get; set; }
        public string PostSharedID { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAdminCreated { get; set; }
        public bool IsPublic { get; set; }
        public bool IsPinned { get; set; }
        public int PinOrder { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public SurveyView Survey { get; set; } = new SurveyView();
        public PollView Poll { get; set; } = new PollView();
        public string UserName { get; set; }
        public string ProfileImageUrl => ConstantUrlPath.ProfileImagePath + UserImageFileId;
        public int LikeCount => Likes.Count;
        public int ShareCount => Shares.Count;
        public int CommentCount { get; set; }
        public int? GroupID { get; set; }
        public string GroupNameEN { get; set; }
        public string GroupNameAR { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string DocumentName { get; set; }
        public int UserImageFileId { get; set; }

        public bool Liked { get; set; }
        public bool IsAmCommented { get; set; }
        public bool Favorited { get; set; }
        public bool IsAmFollowing { get; set; }
        public bool IsAnsweredThisPoll { get; set; }
        public bool IsAdminPostShared { get; set; }

        public PublicProfileView SharedUserDetails { get; set; }

        public string ImageFileUrl
        {
            get
            {
                if (!ImageIDs.Any())
                {
                    return "";
                }

                return ConstantUrlPath.PostFileDownloadPath + ImageIDs.FirstOrDefault();
            }
        }

        public string DocumentFileUrl
        {
            get
            {
                if (!FileIDs.Any())
                {
                    return "";
                }

                return ConstantUrlPath.PostFileDownloadPath + FileIDs.FirstOrDefault();
            }
        }

        public string YouTubeVideoUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(YoutubeVideoID))
                {
                    return "";
                }

                return YoutubeVideoID.Length != 36 ? ConstantUrlPath.YouTubeUrlPath + YoutubeVideoID : "";
            }
        }

        //public string YouTubeVideoUrl { get; set; } = "";

        public string PostVideoUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(YoutubeVideoID))
                {
                    return "";
                }

                return YoutubeVideoID.Length == 36 ? ConstantUrlPath.PostVideoUrlPath + YoutubeVideoID : "";
            }
        }

        //public string PostVideoUrl { get; set; } = "";


    }
    public class SurveyView
	{
		public string ID { get; set; }
		public string Text { get; set; }
		public string LinkURLDescription { get; set; }
		public string RedirectionURL { get; set; }
		public int SurveyID { get; set; }
	}

	public class PollView
	{
        public string ID { get; set; }
		public string Question { get; set; }
        public List<PollAnswerView> Answers { get; set; } = new List<PollAnswerView>();
        public int TotalVotes => Answers.Sum(k => k.Score);

    }

	public class PollAnswerView
	{
		public string ID { get; set; }
		public string Answer { get; set; }
		public int Score { get; set; }
		public IList<int> Users { get; set; }
        public decimal Percentage { get; set; }
	}
}
