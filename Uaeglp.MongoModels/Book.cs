﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Uaeglp.MongoModels
{
	public class Book
	{
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }
		[BsonElement("Name")]
		public string BookName { get; set; }

		public decimal Price { get; set; }

		public string Category { get; set; }

		public string Author { get; set; }
	}
}
