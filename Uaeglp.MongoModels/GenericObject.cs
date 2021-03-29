using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Uaeglp.MongoModels
{
	public class GenericObject
	{
		[BsonId]
		public int ID { get; set; }
		public int TypeID { get; set; }

		[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
		public DateTime Created { get; set; }

		[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
		public DateTime Modified { get; set; }
		public string ModifiedBy { get; set; }
		public string CreatedBy { get; set; }
		public List<MeetupComment> Comments { get; set; } = new List<MeetupComment>();
	}
	public class MeetupComment
	{
		[BsonId]
		public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
		public int UserID { get; set; }
		public string Text { get; set; }
		public int TypeID { get; set; }
		public List<string> FilesIDs { get; set; }
		public List<string> ImagesIDs { get; set; }
	    public string FileName { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsAdminCreated { get; set; }
		public DateTime Created { get; set; } = DateTime.UtcNow;
		public DateTime Modified { get; set; } = DateTime.UtcNow;
		public List<Report> Reports { get; set; } = new List<Report>();
	}

	//public class ChildComment
	//{
	//	[BsonId]
	//	public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
	//	public int UserID { get; set; }
	//	public string Text { get; set; }
	//	public int TypeID { get; set; }
	//	public List<string> FilesIDs { get; set; }
	//	public List<string> ImagesIDs { get; set; }
	//	public bool IsDeleted { get; set; }
	//	public bool IsAdminCreated { get; set; }
	//	public DateTime Created { get; set; } = DateTime.UtcNow;
	//	public DateTime Modified { get; set; } = DateTime.UtcNow;
	//	public List<Report> Reports { get; set; } = new List<Report>();
	//}
}
