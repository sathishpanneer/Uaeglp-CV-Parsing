using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.IdGenerators;
using Uaeglp.Utilities;

namespace Uaeglp.MongoModels
{
	public class Room
	{
        [BsonId]
        public ObjectId ID { get; set; }
		public string RoomTitle { get; set; }
		public int OwnerID { get; set; }
		public int RoomTypeID { get; set; }
		[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
		public DateTime CreatedOn { get; set; }
		public int NumberOfMembers { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
		public DateTime LastModifiedOn { get; set; }
		public List<int> MembersIDs { get; set; } = new List<int>();
		public List<int> ArchivedMembersIDs { get; set; } = new List<int>();
		public IList<Message> Messages { get; set; } = new List<Message>();
		public IList<UnreadMessage> UnreadMessages { get; set; }
		public string ModifiedBy { get; set; }
	}
	public class Message
	{
        [BsonId]
        public ObjectId ID { get; set; }
		public int OwnerID { get; set; }
		public string MessageText { get; set; }
		public int TypeID { get; set; }
		public List<string> FilesIDs { get; set; }
		public List<string> ImagesIDs { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Created { get; set; } = DateTime.UtcNow;
		public List<int> SeenByIDs { get; set; } = new List<int>();
    }
	public class UnreadMessage
	{
        [BsonId]
        public ObjectId? ID { get; set; }
		public int UserID { get; set; }
		public int MessagesCount { get; set; }
	}
}
