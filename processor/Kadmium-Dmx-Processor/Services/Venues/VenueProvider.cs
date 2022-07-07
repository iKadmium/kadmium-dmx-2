using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Models;
using Kadmium_Dmx_Processor.Services.EffectProvider;
using Kadmium_Dmx_Processor.Services.Fixtures;

namespace Kadmium_Dmx_Processor.Services.Venues
{
	public class VenueProvider : IVenueProvider
	{
		private IFixtureDefinitionProvider FixtureDefinitionProvider { get; }
		private IEffectProvider EffectProvider { get; }

		public VenueProvider(IFixtureDefinitionProvider fixtureDefinitionProvider, IEffectProvider effectProvider)
		{
			FixtureDefinitionProvider = fixtureDefinitionProvider;
			EffectProvider = effectProvider;
		}

		public async Task<Venue> GetVenueAsync()
		{
			var definition = await FixtureDefinitionProvider.GetFixtureDefinition("My Manufacturer", "My Model");

			var venue = new Venue(
				"My venue",
				new Dictionary<ushort, Universe>
				{
					{
						1,
						new Universe(
							"My Universe",
							new Dictionary<ushort, Fixture>
							{
								{
									1,
									new Fixture(
										1,
										definition,
										definition.Personalities.Keys.First(),
										"vocalist"
									)
								}
							}
						)
					}
				}
			);

			foreach (var universe in venue.Universes.Values)
			{
				foreach (var fixture in universe.Fixtures.Values)
				{
					fixture.Effects.AddRange(EffectProvider.GetEffects(fixture.Definition, fixture.Personality));
					fixture.EffectRenderers.AddRange(EffectProvider.GetEffectRenderers(fixture.Definition, fixture.Personality));
				}
			}

			return venue;
		}
	}
}