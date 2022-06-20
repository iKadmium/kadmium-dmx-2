using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Models;

namespace Kadmium_Dmx_Processor.Services.Fixtures
{
	public class FixtureDefinitionProvider : IFixtureDefinitionProvider
	{
		public Task<FixtureDefinition> GetFixtureDefinition(string manufacturer, string model)
		{
			var definition = new FixtureDefinition(
				"My Manufacturer",
				"My Model",
				new Dictionary<string, FixturePersonalityDefinition>
				{
					{
						"3ch",
						new FixturePersonalityDefinition(
							"3ch",
							new Dictionary<ushort, DmxChannel>
							{
								{ 1, new DmxChannel("Red", 1) },
								{ 2, new DmxChannel("Green", 2) },
								{ 3, new DmxChannel("Blue", 3) },
							}
						)
					}
				},
				new Dictionary<string, Axis> { }
			);
			return Task.FromResult(definition);
		}
	}
}