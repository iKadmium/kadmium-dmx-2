using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Webapi.Models;

namespace Webapi.Services
{
	public class MongoDefaultsProvider : IDefaultsProvider
	{
		public const string DefaultsId = "1234";

		private IMongoDatabase Db { get; }
		private IMongoCollection<Defaults> DefaultsCollection { get; }

		public MongoDefaultsProvider(IMongoDatabase db)
		{
			Db = db;
			DefaultsCollection = Db.GetCollection<Defaults>("defaults");
		}

		public async Task<Defaults> GetDefaults()
		{
			var defaults = await DefaultsCollection.Find(
				Builders<Defaults>.Filter.Where(x => x.Id == DefaultsId)
			).ToListAsync();

			switch (defaults.Count)
			{
				case 1:
					return defaults.Single();
				default:
					return new Defaults() { Id = DefaultsId };
			}
		}

		public async Task<Defaults> UpdateDefaults(Defaults defaults)
		{
			var result = await DefaultsCollection.FindOneAndReplaceAsync(
				Builders<Defaults>.Filter.Where(x => x.Id == DefaultsId),
				defaults,
				new() { IsUpsert = true }
			);

			return result;
		}
	}
}