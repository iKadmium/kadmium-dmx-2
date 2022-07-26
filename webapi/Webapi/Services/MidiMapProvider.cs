using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;
using MongoDB.Driver;
using Webapi.Models;

namespace Webapi.Services
{
	public class MidiMapProvider : MongoCrudProvider<MidiMapKey, MidiMap>, IMidiMapProvider
	{
		private IMqttConnectionProvider Mqtt { get; }

		public MidiMapProvider(IMongoDatabase db, IMqttConnectionProvider mqtt, ILogger<MongoCrudProvider<MidiMapKey, MidiMap>> logger) : base(db, logger)
		{
			Mqtt = mqtt;
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

		public async Task Activate(string id)
		{
			try
			{
				var map = await Read(id);
				await Mqtt.PublishAsync("/midimap/load", JsonSerializer.SerializeToUtf8Bytes(map));
			}
			catch (Exception e)
			{
				Logger.LogError($"Failed to activate MIDI Map {id}", e);
			}
		}
	}
}