using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;
using MongoDB.Driver;
using Webapi.Models;

namespace Webapi.Services
{
	public class FixtureDefinitionProvider : MongoCrudProvider<FixtureDefinitionKey, FixtureDefinition>
	{
		public FixtureDefinitionProvider(IMongoDatabase db) : base(db)
		{
		}

		protected override ProjectionDefinition<FixtureDefinition> KeyProjection { get; } = Builders<FixtureDefinition>.Projection
			.Exclude(x => x.MovementAxis)
			.Exclude(x => x.Personalities);

		protected override string CollectionName { get; } = "fixtureDefinitions";

		protected override FilterDefinition<FixtureDefinition> GetSearchFilter(string query)
		{
			return Builders<FixtureDefinition>.Filter.Where(x =>
				x.Manufacturer.Contains(query, StringComparison.OrdinalIgnoreCase)
				|| x.Model.Contains(query, StringComparison.OrdinalIgnoreCase)
			);
		}
	}
}