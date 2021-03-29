using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Uaeglp.MongoModels
{
	public class TimeStampModel
	{
		public int UserID { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
		public DateTime Datetime { get; set; } = DateTime.UtcNow;
	}
}
