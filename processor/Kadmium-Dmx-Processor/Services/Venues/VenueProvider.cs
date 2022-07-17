using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Shared.Models;
using Kadmium_Dmx_Processor.Services.EffectProvider;
using Kadmium_Dmx_Processor.Services.Groups;
using Kadmium_Dmx_Processor.Services.Mqtt;

namespace Kadmium_Dmx_Processor.Services.Venues
{
	public class VenueProvider : IVenueProvider
	{
		private IEffectProvider EffectProvider { get; }
		private IGroupProvider GroupProvider { get; }
		public Venue Venue { get; private set; } = new Venue("Empty Venue", "No City", new Dictionary<ushort, Universe>());
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
			GroupProvider.Groups.Clear();
			await MqttProvider.UnsubscribeAll();

			var venuePayload = JsonSerializer.Deserialize<VenuePayload>(document, JsonSerializerOptions);
			if (venuePayload == null)
			{
				throw new ArgumentException("The given Json document was not valud");
			}

			Venue = venuePayload.Venue;
			foreach (var groupName in venuePayload.Groups)
			{
				GroupProvider.Groups.Add(groupName, new Group());
			}

			foreach (var universe in Venue.Universes)
			{
				var universeActor = new UniverseActor(universe.Value);
				UniverseActors.Add(universe.Key, universeActor);

				foreach (var fixtureInstance in universe.Value.Fixtures)
				{
					var definition = venuePayload.FixtureDefinitions.Single(x =>
						x.Manufacturer == fixtureInstance.Value.Manufacturer
						&& x.Model == fixtureInstance.Value.Model);
					var fixtureActor = new FixtureActor(fixtureInstance.Value, definition);
					fixtureActor.EffectRenderers.AddRange(EffectProvider.GetEffectRenderers(fixtureActor, fixtureInstance.Key));
					fixtureActor.Effects.AddRange(EffectProvider.GetEffects(fixtureActor));
					universeActor.FixtureActors.Add(fixtureInstance.Key, fixtureActor);
					foreach (var groupName in fixtureInstance.Value.Groups)
					{
						var locator = new FixtureLocator(universe.Key, fixtureInstance.Key);
						GroupProvider.Groups[groupName].Fixtures.Add(locator, fixtureActor);
					}
				}
			}

			foreach (var group in GroupProvider.Groups.Values)
			{
				EffectProvider.GetEffects(group);
			}

			var groupTopics = from groupKvp in GroupProvider.Groups
							  from effectAttribute in groupKvp.Value.EffectAttributes
							  where !effectAttribute.Value.InternalOnly
							  select $"/group/{groupKvp.Key}/{effectAttribute.Key}";

			var groupFixtureTopics = from groupKvp in GroupProvider.Groups
									 from universeKvp in UniverseActors
									 from fixtureKvp in universeKvp.Value.FixtureActors
									 from effectAttribute in fixtureKvp.Value.EffectAttributes
									 where !effectAttribute.Value.InternalOnly
									 select $"/group/{groupKvp.Key}/{effectAttribute.Key}";

			var fixtureTopics = from universeKvp in UniverseActors
								from fixtureKvp in universeKvp.Value.FixtureActors
								from effectAttribute in fixtureKvp.Value.EffectAttributes
								where !effectAttribute.Value.InternalOnly
								select $"/fixture/{universeKvp.Key}/{fixtureKvp.Key}/{effectAttribute.Key}";

			var topicPromises = groupTopics
				.Union(groupFixtureTopics)
				.Union(fixtureTopics);

			await MqttProvider.Subscribe(topicPromises);
		}
	}
}