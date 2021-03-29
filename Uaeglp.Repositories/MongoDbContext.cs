using MongoDB.Bson;
using MongoDB.Driver;
using Uaeglp.MongoModels;

namespace Uaeglp.Repositories
{
	public partial class MongoDbContext
	{
		public IMongoDatabase Database { get; private set; }
		public IMongoCollection<Post> Posts { get; private set; }
		public IMongoCollection<User> Users { get; private set; }
        public IMongoCollection<Room> Rooms { get; private set; }
		public IMongoCollection<NotificationGenericObject> NotificationGenericObjects { get; private set; }
		public IMongoCollection<BsonDocument> GridFsFiles { get; private set; }
		public IMongoCollection<GenericObject> GenericObjects { get; private set; }
		


		public MongoDbContext(string server, string database)
		{
			var client = new MongoClient(server);
			Database = client.GetDatabase(database);
			Posts = Database.GetCollection<Post>("Post");
			Users = Database.GetCollection<User>("User");
			Rooms = Database.GetCollection<Room>("Room");
			NotificationGenericObjects = Database.GetCollection<NotificationGenericObject>("NotificationGenericObject");
            GridFsFiles = Database.GetCollection<BsonDocument>("fs.files");
			GenericObjects = Database.GetCollection<GenericObject>("GenericObject");
		}
	}


}
