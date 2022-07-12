using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Processor.Models;
using Kadmium_Dmx_Processor.Services.EffectProvider;
using Kadmium_Dmx_Processor.Services.Groups;
using Kadmium_Dmx_Processor.Services.Mqtt;

namespace Kadmium_Dmx_Processor.Services.Venues
{
	public class VenueProvider : IVenueProvider
	{
		private IEffectProvider EffectProvider { get; }
		private IGroupProvider GroupProvider { get; }
		public Venue Venue { get; private set; } = new Venue("Empty Venue", new Dictionary<ushort, Universe>());
		public Dictionary<ushort, UniverseActor> UniverseActors { get; }
		private IMqttProvider MqttProvider { get; }
		private JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		public VenueProvider(IEffectProvider effectProvider, IGroupProvider groupProvider, IMqttProvider mqttProvider)
		{
			EffectProvider = effectProvider;
			GroupProvider = groupProvider;
			UniverseActors = new Dictionary<ushort, UniverseActor>();
			MqttProvider = mqttProvider;
		}

		public async void LoadVenue(JsonDocument document)
		{
			foreach (var universe in UniverseActors.Values)
			{
				universe.Dispose();
			}
			UniverseActors.Clear();
			GroupProvider.Clear();
			await MqttProvider.UnsubscribeAll();

			var venueElement = document.RootElement.GetProperty("venue");
			var fixtureDefinitionsElement = document.RootElement.GetProperty("fixtureDefinitions");
			var fixtureDefinitions = JsonSerializer.Deserialize<List<FixtureDefinition>>(fixtureDefinitionsElement, JsonSerializerOptions) ?? new List<FixtureDefinition>();
			Venue = JsonSerializer.Deserialize<Venue>(venueElement, JsonSerializerOptions) ?? Venue;

			foreach (var universe in Venue.Universes)
			{
				var universeActor = new UniverseActor(universe.Value);
				UniverseActors.Add(universe.Key, universeActor);

				foreach (var fixtureInstance in universe.Value.Fixtures)
				{
					var definition = fixtureDefinitions.Single(x =>
						x.Manufacturer == fixtureInstance.Value.Manufacturer
						&& x.Model == fixtureInstance.Value.Model);
					var fixtureActor = new FixtureActor(fixtureInstance.Value, definition);
					fixtureActor.EffectRenderers.AddRange(EffectProvider.GetEffectRenderers(fixtureActor));
					fixtureActor.Effects.AddRange(EffectProvider.GetEffects(fixtureActor));
					universeActor.FixtureActors.Add(fixtureInstance.Key, fixtureActor);
				}
			}

			var groupElement = document.RootElement.GetProperty("groups");
			var universeMappings = JsonSerializer.Deserialize<Dictionary<ushort, Dictionary<ushort, string>>>(groupElement, JsonSerializerOptions) ?? new Dictionary<ushort, Dictionary<ushort, string>>();

			foreach (var universeMapping in universeMappings)
			{
				var universe = UniverseActors[universeMapping.Key];
				foreach (var fixtureMapping in universeMapping.Value)
				{
					var fixture = universe.FixtureActors[fixtureMapping.Key];
					var groupName = fixtureMapping.Value;
					GroupProvider.Groups[groupName].Fixtures.Add(fixture);
				}
			}


			foreach (var group in GroupProvider.Groups.Values)
			{
				EffectProvider.GetEffects(group);
			}
			var groupTopics = from groupKvp in GroupProvider.Groups
							  from attributeName in groupKvp.Value.EffectAttributes.Keys
							  select $"/group/{groupKvp.Key}/{attributeName}";

			var groupFixtureTopics = from groupKvp in GroupProvider.Groups
									 from universeKvp in UniverseActors
									 from fixtureKvp in universeKvp.Value.FixtureActors
									 from effectAttribute in fixtureKvp.Value.EffectAttributes
									 select $"/group/{groupKvp.Key}/{effectAttribute.Key}";

			var fixtureTopics = from universeKvp in UniverseActors
								from fixtureKvp in universeKvp.Value.FixtureActors
								from effectAttribute in fixtureKvp.Value.EffectAttributes
								select $"/fixture/{universeKvp.Key}/{fixtureKvp.Key}/{effectAttribute.Key}";

			var topicPromises = groupTopics
				.Union(groupFixtureTopics)
				.Union(fixtureTopics);
			await MqttProvider.Subscribe(topicPromises);
		}
	}
}