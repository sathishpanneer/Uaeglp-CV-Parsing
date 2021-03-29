using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Uaeglp.MongoModels
{
	public class Survey
	{
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public ObjectId Id { get; set; }
		public string Text { get; set; }
		public string LinkURLDescription { get; set; }
		public string RedirectionURL { get; set; }
		public int SurveyID { get; set; }
	}
}
