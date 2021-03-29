using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Uaeglp.MongoModels
{

    public class NotificationGenericObject
    {
        public List<Notification> Notifications = new List<Notification>();

        [BsonId]
        public ObjectId ID { get; set; }

        public int UserID { get; set; }

        public int UnseenNotificationCounter { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Modified { get; set; } = DateTime.Now;
    }

}
