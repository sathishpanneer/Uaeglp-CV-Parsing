using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Uaeglp.MongoModels
{
	public class Notification
	{
        [BsonId]
        public ObjectId ID { get; set; }
        public string ParentID { get; set; }
        public int ParentTypeID { get; set; }
        public int ActionID { get; set; }
        public int SenderID { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Modified { get; set; } = DateTime.Now;
        public bool IsRead { get; set; }
        public bool GeneralNotification { get; set; }
        public bool IsPushed { get; set; }
        //public int PostTypeID { get; set; }
    }
}
