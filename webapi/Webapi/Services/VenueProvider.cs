using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;
using MongoDB.Driver;
using Webapi.Models;

namespace Webapi.Services
{
	public class VenueProvider : MongoCrudProvider<VenueKey, Venue>
	{
		public VenueProvider(IMongoDatabase db) : base(db)
		{
		}

		protected override ProjectionDefinition<Venue> KeyProjection { get; } = Builders<Venue>.Projection.Exclude(x => x.Universes);

		protected override string CollectionName { get; } = "venues";

		protected override FilterDefinition<Venue> GetSearchFilter(string query)
		{
			return Builders<Venue>.Filter.Where(x =>
				x.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
				|| x.City.Contains(query, StringComparison.OrdinalIgnoreCase)
			);
		}
	}
}