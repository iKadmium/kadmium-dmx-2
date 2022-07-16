using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Shared.Models;
using Kadmium_Dmx_Processor.Utilities;

namespace Kadmium_Dmx_Processor.EffectRenderers.Movement
{
	public class Movement16BitRenderer : IEffectRenderer
	{
		public IEnumerable<DmxChannel> RenderTargets { get; }
		public EffectAttribute AxisAttribute { get; }
		private ushort CoarseAddress { get; }
		private ushort FineAddress { get; }

		public Movement16BitRenderer(FixtureActor actor, Axis axis, string name, ushort fixtureAddress)
		{
			var coarse = actor.Channels.Values.Single(x => x.Name == $"{name}Coarse");
			var fine = actor.Channels.Values.Single(x => x.Name == $"{name}Fine");
			RenderTargets = new[] { coarse, fine };

			AxisAttribute = actor.AddAttribute(name);

			CoarseAddress = (ushort)(fixtureAddress + actor.Channels.Single(x => x.Value == coarse).Key - 1);
			FineAddress = (ushort)(fixtureAddress + actor.Channels.Single(x => x.Value == fine).Key - 1);
		}

		public void Render(Memory<byte> dmxMemory)
		{
			var rawValue = AxisAttribute.Value;
			var shortVal = (ushort)Scale.Rescale(rawValue, 0, 1, ushort.MinValue, ushort.MaxValue);
			var bytes = BitConverter.GetBytes(shortVal);
			if (BitConverter.IsLittleEndian)
			{
				dmxMemory.Span[CoarseAddress] = bytes[1];
				dmxMemory.Span[FineAddress] = bytes[0];
			}
			else
			{
				dmxMemory.Span[CoarseAddress] = bytes[0];
				dmxMemory.Span[FineAddress] = bytes[1];
			}
		}
	}
}