using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Shared.Models;

namespace Kadmium_Dmx_Processor.Test
{
	public static class FixtureHelper
	{
		public static FixtureActor GetFixture(params string[] channelNames)
		{
			var channels = Enumerable
				.Range(1, channelNames.Length)
				.ToDictionary(
					(x) => (ushort)x,
					(x) => new DmxChannel(channelNames[x - 1])
				);

			var definition = new FixtureDefinition(
				"Someone",
				"Something",
				new Dictionary<string, Dictionary<ushort, DmxChannel>>
				{
					{
						"Personality",
						new Dictionary<ushort, DmxChannel>(channels)
					}
				},
				new Dictionary<string, Axis> { }
			);
			var fixtureInstance = new FixtureInstance("Someone", "Something", "Personality");
			var actor = new FixtureActor(fixtureInstance, definition);

			return actor;
		}

		public static FixtureActor GetMovingFixture(params string[] axisNames)
		{
			var channels = Enumerable
				.Range(1, axisNames.Length)
				.ToDictionary(
					(x) => (ushort)x,
					(x) => new DmxChannel(axisNames[x - 1])
				);

			var axis = axisNames.ToDictionary(x => x, x => new Axis(-90, 90));

			var definition = new FixtureDefinition(
				"Someone",
				"Something",
				new Dictionary<string, Dictionary<ushort, DmxChannel>>
				{
					{
						"Personality",
						channels
					}
				},
				axis
			);
			var fixtureInstance = new FixtureInstance("Someone", "Something", "Personality");
			var actor = new FixtureActor(fixtureInstance, definition);

			return actor;
		}
	}
}