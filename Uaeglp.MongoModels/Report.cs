using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Uaeglp.MongoModels
{
	public class Report
	{
        [BsonId]
        public ObjectId ID { get; set; }
		public int UserID { get; set; }
		public int ReasonTypeID { get; set; }
		public string Reason { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
	}
}
