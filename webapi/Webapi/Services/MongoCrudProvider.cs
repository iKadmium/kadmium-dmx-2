using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Webapi.Services
{
	public abstract class MongoCrudProvider<TKey, TObject> : ICrudProvider<TKey, TObject>
		where TKey : IHasId
		where TObject : IHasId
	{
		private IMongoDatabase Db { get; }
		private IMongoCollection<TObject> Collection => Db.GetCollection<TObject>(CollectionName);
		protected abstract ProjectionDefinition<TObject> KeyProjection { get; }
		protected abstract string CollectionName { get; }

		public MongoCrudProvider(IMongoDatabase db)
		{
			Db = db;
		}

		public async Task<string> Create(TObject obj)
		{
			obj.Id = Guid.NewGuid().ToString();
			await Collection.InsertOneAsync(obj);
			return obj.Id;
		}

		public async Task<IEnumerable<TKey>> ReadKeys()
		{
			var result = await Collection
				.Find(Builders<TObject>.Filter.Empty)
				.Project(KeyProjection)
				.ToListAsync();

			var keys = result.Select(x => BsonSerializer.Deserialize<TKey>(x));
			return keys;
		}

		public async Task<TObject> Read(string id)
		{
			var result = await Collection
				.Find(Builders<TObject>.Filter.Where(x => x.Id == id))
				.ToListAsync();

			var obj = result.Single();
			return obj;
		}

		public async Task<TObject> Update(string id, TObject obj)
		{
			var result = await Collection.FindOneAndReplaceAsync(
				Builders<TObject>.Filter.Where(x => x.Id == id),
				obj
			);

			return result;
		}

		public async Task Delete(string id)
		{
			var result = await Collection.FindOneAndDeleteAsync(
				Builders<TObject>.Filter.Where(x => x.Id == id)
			);
		}

		public async Task<IEnumerable<TKey>> Search(string query)
		{
			var result = await Collection
				.Find(GetSearchFilter(query))
				.Project(KeyProjection)
				.ToListAsync();

			var keys = result.Select(x => BsonSerializer.Deserialize<TKey>(x));
			return keys;
		}

		protected abstract FilterDefinition<TObject> GetSearchFilter(string query);
	}
}