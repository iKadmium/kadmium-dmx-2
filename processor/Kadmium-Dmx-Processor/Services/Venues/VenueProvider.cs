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

namespace Kadmium_Dmx_Processor.Services.Venues
{
	public class VenueProvider : IVenueProvider
	{
		private IEffectProvider EffectProvider { get; }
		private IGroupProvider GroupProvider { get; }
		public Venue Venue { get; private set; } = new Venue("Empty Venue", new Dictionary<ushort, Universe>());
		public Dictionary<ushort, UniverseActor> UniverseActors { get; }
		private JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		public VenueProvider(IEffectProvider effectProvider, IGroupProvider groupProvider)
		{
			EffectProvider = effectProvider;
			GroupProvider = groupProvider;
			UniverseActors = new Dictionary<ushort, UniverseActor>();
		}

		public void LoadVenue(JsonDocument document)
		{
			foreach (var universe in UniverseActors.Values)
			{
				universe.Dispose();
			}
			UniverseActors.Clear();

			GroupProvider.Clear();
			//var fixtureDefinitions = document.RootElement.GetProperty("fixtureDefinitions");
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
					fixtureActor.Effects.AddRange(EffectProvider.GetEffects(fixtureActor));
					fixtureActor.EffectRenderers.AddRange(EffectProvider.GetEffectRenderers(fixtureActor));
					universeActor.FixtureActors.Add(fixtureInstance.Key, fixtureActor);
				}
			}

			// foreach (var currentGroup in GroupProvider.Groups.Values)
			// {
			// 	var fixturesInGroup = from universe in Venue.Universes.Values
			// 						  from fixture in universe.Fixtures.Values
			// 						  where fixture.Group == currentGroup.Name
			// 						  select fixture;

			// 	foreach (var fixture in fixturesInGroup)
			// 	{
			// 		currentGroup.Fixtures.Add(new FixtureActor(fixture));
			// 	}
			// }
		}
	}
}