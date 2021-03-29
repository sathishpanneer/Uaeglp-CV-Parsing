using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Uaeglp.MongoModels
{
	public class Post
	{

        [BsonId]
        public ObjectId ID { get; set; } //= ObjectId.GenerateNewId();

        public List<string> ImageIDs { get; set; } = new List<string>();
        public List<string> FileIDs { get; set; } = new List<string>();
        public List<Report> Reports { get; set; } = new List<Report>();
        public List<int> Likes { get; set; } = new List<int>();
        public List<int> Favorites { get; set; } = new List<int>();
        public List<int> Dislikes { get; set; } = new List<int>();
        public List<int> Shares { get; set; } = new List<int>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<TimeStampModel> LikesTimestamp { get; set; } = new List<TimeStampModel>();
        public List<TimeStampModel> FavoritesTimestamp { get; set; } = new List<TimeStampModel>();
        public List<TimeStampModel> SharesTimestamp { get; set; } = new List<TimeStampModel>();
        
        public int UserID { get; set; }

        public int TypeID { get; set; }

        public DateTime Modified { get; set; } = DateTime.Now;

        public DateTime Created { get; set; } = DateTime.Now;

        public string ModifiedBy { get; set; }

        public string CreatedBy { get; set; }

        public string Text { get; set; }

        public string YoutubeVideoID { get; set; } = "";

        public int InteractionCounter { get; set; }

        public ObjectId PostSharedID { get; set; }

        public bool IsGroupCreated { get; set; }

        public int? GroupID { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsAdminCreated { get; set; }

        public bool IsPinned { get; set; }

        public int PinOrder { get; set; }

        public bool IsPublic { get; set; }

        public PollPost PollPost { get; set; } = new PollPost();
        public Survey SurveyPost { get; set; } = new Survey();

        //public string Category { get; set; }
        //public string Author { get; set; }
    }
}
