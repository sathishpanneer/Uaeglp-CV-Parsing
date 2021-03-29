using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Uaeglp.MongoModels
{
	public class PollPost
	{
        public IList<PollAnswer> Answers { get; set; } = new List<PollAnswer>();

		[BsonId]
		public ObjectId Id { get; set; }
		
		public string Question { get; set; }
	}

	public class PollAnswer
	{
        [BsonId]
		public ObjectId Id { get; set; }
		public string Answer { get; set; }
		public int Score { get; set; }
		public List<int> Users { get; set; } = new List<int>();
	}
}
