using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Uaeglp.Utilities;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.ViewModels
{
	public class NotificationGenericObjectView
	{
       
        public string ID { get; set; }

        public int UserID { get; set; }

        public int UnseenNotificationCounter { get; set; }

        public List<NotificationView> NotificationsList { get; set; } = new List<NotificationView>();

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Modified { get; set; } = DateTime.Now;
    }

	public class NotificationView
    {
        private string _redirectionPath;
        [JsonProperty(PropertyName = "id")]
		public string Id { get; set; }
        [JsonProperty(PropertyName = "parentID")]
        public string ParentID { get; set; }
        [JsonProperty(PropertyName = "parentTypeID")]
        public ParentType ParentTypeID { get; set; }
        [JsonProperty(PropertyName = "postTypeID")]
        public PostType PostTypeID { get; set; }
        [JsonProperty(PropertyName = "actionID")]
        public ActionType ActionID { get; set; }
        [JsonProperty(PropertyName = "senderID")]
        public int SenderID { get; set; }
        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; } = DateTime.Now;
        [JsonProperty(PropertyName = "modified")]
        public DateTime Modified { get; set; } = DateTime.Now;
        [JsonProperty(PropertyName = "isRead")]
        public bool IsRead { get; set; }
        [JsonProperty(PropertyName = "generalNotification")]
        public bool GeneralNotification { get; set; }
        [JsonProperty(PropertyName = "userNameEn")]
        public string UserNameEn { get; set; }
        [JsonProperty(PropertyName = "userNameAr")]
        public string UserNameAr { get; set; }
        [JsonProperty(PropertyName = "userImageFileId")]
        public int UserImageFileId { get; set; }
        [JsonProperty(PropertyName = "profileImageUrl")]
        public string ProfileImageUrl => ConstantUrlPath.ProfileImagePath + UserImageFileId;
        [JsonProperty(PropertyName = "postImageUrl")]
        public string PostImageUrl =>
        (ParentTypeID == ParentType.Post && PostTypeID != PostType.Image) ? ConstantUrlPath.PostImageDownloadPath : (ParentTypeID == ParentType.Post && PostTypeID == PostType.Image)? ConstantUrlPath.PostImageDownloadPath + ParentID: "";
        [JsonProperty(PropertyName = "redirectUrlPath")]
        public string RedirectUrlPath
        {
            get
            {
                switch (ParentTypeID)
                {

                    case ParentType.Post:
                        return $"/api/Social/get-post/{ParentID}/{SenderID}";
                        break;
                    case ParentType.User:
                        //return $"/api/Profile/get-public-profile/{SenderID}/{PublicProfileId}";
                        break;
                    case ParentType.Group:
                        break;
                    case ParentType.KnowledgeHub:
                        break;
                    case ParentType.Challenge:
                        break;
                    case ParentType.EngagementActivities:
                        break;
                    case ParentType.Batch:
                        break;
                    case ParentType.Event:
                        break;
                    case ParentType.Assessment:
                        break;
                    case ParentType.Meetup:
                        break;
                    case ParentType.AssessmentGroup:
                        break;
                    case ParentType.AssignedAssessment:
                        break;
                }

                return _redirectionPath;
            }
            set => _redirectionPath = value;
        }

        [JsonProperty(PropertyName = "OwnerId")]
        public int OwnerID { get; set; }
        public string titleEn { get; set; }
        public string titleAr { get; set; }
        public int? categoryID { get; set; }


    }
}
