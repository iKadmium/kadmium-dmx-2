using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Webapi.Services
{
	public interface IMongoProvider
	{
		Task Add<TObject>(string collectionName, TObject obj);
		Task<IEnumerable<TKey>> ReadKeys<TObject, TKey>(string collectionName, ProjectionDefinition<TObject> projection);
		Task<TObject> Read<TObject>(string collectionName, string id);
		Task<TObject> Update<TObject>(string collectionName, string id, TObject obj);
		Task Delete(string collectionName, string id);
	}
}