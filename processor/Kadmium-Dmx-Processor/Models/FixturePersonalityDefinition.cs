using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Models
{
	public class FixturePersonalityDefinition
	{
		public FixturePersonalityDefinition(string name, Dictionary<ushort, DmxChannel> channels)
		{
			Name = name;
			Channels = channels;
		}

		public string Name { get; }
		public Dictionary<ushort, DmxChannel> Channels { get; }
	}
}