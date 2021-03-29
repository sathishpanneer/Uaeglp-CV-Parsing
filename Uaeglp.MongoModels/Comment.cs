using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Uaeglp.MongoModels
{
	public class Comment
	{
        [BsonId]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public int UserID { get; set; }
		public string Text { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsAdminCreated { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Modified { get; set; } = DateTime.UtcNow;
		public List<Report> Reports { get; set; } = new List<Report>();
	}
}
