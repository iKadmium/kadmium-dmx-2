using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;
using MongoDB.Driver;
using Webapi.Models;

namespace Webapi.Services
{
	public class VenueProvider : MongoCrudProvider<VenueKey, Venue>, IVenueProvider
	{
		private IMqttConnectionProvider Mqtt { get; }
		private IFixtureDefinitionProvider FixtureDefinitionProvider { get; }

		public VenueProvider(IMongoDatabase db, IMqttConnectionProvider mqtt, IFixtureDefinitionProvider fixtureDefinitionProvider, ILogger<MongoCrudProvider<VenueKey, Venue>> logger) : base(db, logger)
		{
			Mqtt = mqtt;
			FixtureDefinitionProvider = fixtureDefinitionProvider;
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

		protected override IOrderedFindFluent<Venue, Venue> Sort(IFindFluent<Venue, Venue> find)
		{
			return find
				.SortBy(x => x.City)
				.ThenBy(x => x.Name);
		}

		public async Task Activate(string id)
		{
			try
			{
				var venue = await Read(id);
				var instances = venue.Universes.Values.SelectMany(x => x.Fixtures.Values);
				var ids = await GetIdsForInstances(instances);
				var definitions = await FixtureDefinitionProvider.Read(ids);
				var groups = instances.SelectMany(x => x.Groups).Distinct();

				var payload = new VenuePayload(groups, definitions, venue);
				var serialized = JsonSerializer.SerializeToUtf8Bytes(payload, new JsonSerializerOptions(JsonSerializerDefaults.Web));
				await Mqtt.PublishAsync("/venue/load", serialized, true);
			}
			catch (Exception e)
			{
				Logger.LogError(e, $"Failed to activate venue {id}");
			}
		}

		private async Task<IEnumerable<string>> GetIdsForInstances(IEnumerable<FixtureInstance> instances)
		{
			var allKeys = await FixtureDefinitionProvider.ReadKeys();
			var ids = new List<string>();
			foreach (var instance in instances)
			{
				var key = allKeys.Single(key => key.Manufacturer == instance.Manufacturer && key.Model == instance.Model);
				if (key != null && key.Id != null && !ids.Contains(key.Id))
				{
					ids.Add(key.Id);
				}
			}
			return ids;
		}
	}
}