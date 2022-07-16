using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Shared.Models
{
	public class VenuePayload
	{
		public VenuePayload(IEnumerable<string> groups, IEnumerable<FixtureDefinition> fixtureDefinitions, Venue venue)
		{
			Groups = groups;
			FixtureDefinitions = fixtureDefinitions;
			Venue = venue;
		}

		public IEnumerable<string> Groups { get; }
		public IEnumerable<FixtureDefinition> FixtureDefinitions { get; }
		public Venue Venue { get; }

	}
}