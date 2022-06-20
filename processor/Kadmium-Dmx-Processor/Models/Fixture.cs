using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Models
{
	public class Fixture
	{
		public Fixture(ushort address, FixtureDefinition definition, string personality, string group)
		{
			Address = address;
			Channels = new Dictionary<ushort, DmxChannel>();
			foreach (var channel in definition.Personalities[personality].Channels.Values)
			{
				Channels.Add(channel.Address, new DmxChannel(channel));
			}
			MovementAxis = new Dictionary<string, Axis>();
			foreach (var axis in definition.MovementAxis.Values)
			{
				MovementAxis.Add(axis.Name, new Axis(axis));
			}
			Group = group;
		}

		public ushort Address { get; }
		public Dictionary<ushort, DmxChannel> Channels { get; } = new Dictionary<ushort, DmxChannel>();
		public Dictionary<string, Axis> MovementAxis { get; }
		public string Group { get; }
	}
}