using MongoDB.Driver;
using System.Collections.Generic;
using Uaeglp.MongoModels;
using Uaeglp.Repositories;

namespace Uaeglp.Services
{
	public class MongoService
	{
		private MongoDbContext _mongoDbContext = new MongoDbContext("mongodb://localhost:27017", "GLP");
		public MongoService()
		{

		}

		//public List<Book> Get() =>
		//	_mongoDbContext.Books.Find(book => true).ToList();
	}
}
