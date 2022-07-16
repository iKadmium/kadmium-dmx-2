using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects;
using Kadmium_Dmx_Shared.Models;
using Kadmium_Dmx_Processor.Utilities;

namespace Kadmium_Dmx_Processor.EffectRenderers
{
	public class DmxChannelRenderer : IEffectRenderer
	{
		public EffectAttribute Attribute { get; }
		public IEnumerable<DmxChannel> RenderTargets { get; }
		private ushort Address { get; }

		public DmxChannelRenderer(FixtureActor actor, DmxChannel channel, ushort fixtureAddress)
		{
			Attribute = actor.AddAttribute(channel.Name);
			RenderTargets = new[] { channel };
			Address = (ushort)(actor.Channels.Single(x => x.Value == channel).Key + fixtureAddress - 1);
		}

		public void Render(Memory<byte> dmxMemory)
		{
			var rawValue = Attribute.Value;
			byte result = (byte)Scale.Rescale(rawValue, 0, 1, 0, 255);
			dmxMemory.Span[Address] = result;
		}
	}
}