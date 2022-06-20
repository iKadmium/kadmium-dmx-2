using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Models;
using Kadmium_Dmx_Processor.Services.Fixtures;

namespace Kadmium_Dmx_Processor.Services.Venues
{
	public class VenueProvider : IVenueProvider
	{
		private IFixtureDefinitionProvider FixtureDefinitionProvider { get; }

		public VenueProvider(IFixtureDefinitionProvider fixtureDefinitionProvider)
		{
			FixtureDefinitionProvider = fixtureDefinitionProvider;
		}

		public async Task<Venue> GetVenueAsync()
		{
			var definition = await FixtureDefinitionProvider.GetFixtureDefinition("My Manufacturer", "My Model");

			return new Venue(
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
		}
	}
}