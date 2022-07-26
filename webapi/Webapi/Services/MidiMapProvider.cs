using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;
using MongoDB.Driver;
using Webapi.Models;

namespace Webapi.Services
{
	public class MidiMapProvider : MongoCrudProvider<MidiMapKey, MidiMap>
	{
		public MidiMapProvider(IMongoDatabase db) : base(db)
		{
		}

		protected override ProjectionDefinition<MidiMap> KeyProjection => Builders<MidiMap>
			.Projection
			.Include(x => x.Id)
			.Include(x => x.Name);

		protected override string CollectionName { get; } = "midiMaps";

		protected override FilterDefinition<MidiMap> GetSearchFilter(string query)
		{
			return Builders<MidiMap>.Filter.Where(x =>
				x.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
			);
		}

		protected override IOrderedFindFluent<MidiMap, MidiMap> Sort(IFindFluent<MidiMap, MidiMap> find)
		{
			return find.SortBy(x => x.Name);
		}
	}
}