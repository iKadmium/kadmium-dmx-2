using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.EffectRenderers;
using Kadmium_Dmx_Processor.Effects;

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
			Personality = personality;
			Definition = definition;
		}

		public ushort Address { get; }
		public Dictionary<ushort, DmxChannel> Channels { get; } = new Dictionary<ushort, DmxChannel>();
		public Dictionary<string, Axis> MovementAxis { get; }
		public string Group { get; }
		public Dictionary<string, EffectAttribute> Pipeline { get; } = new Dictionary<string, EffectAttribute>();
		public List<IEffectRenderer> EffectRenderers { get; } = new List<IEffectRenderer>();
		public List<IEffect> Effects { get; } = new List<IEffect>();
		public string Personality { get; }
		public FixtureDefinition Definition { get; }
	}
}