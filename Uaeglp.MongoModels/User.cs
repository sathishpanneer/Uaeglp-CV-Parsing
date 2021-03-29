using MongoDB.Bson;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Uaeglp.MongoModels
{
	public class User
	{
        [BsonId]
        public int Id { get; set; }
		public List<int> FollowersIDs { get; set; } = new List<int>();
		public List<int> FollowingIDs { get; set; } = new List<int>();
		public List<int> MyFavouriteProfilesIDs { get; set; } = new List<int>();
		public IList<Viewer> Viewers { get; set; } = new List<Viewer>();
	}
}
