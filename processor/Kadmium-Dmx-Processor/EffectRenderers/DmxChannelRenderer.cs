using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects;
using Kadmium_Dmx_Processor.Models;
using Kadmium_Dmx_Processor.Utilities;

namespace Kadmium_Dmx_Processor.EffectRenderers
{
	public class DmxChannelRenderer : IEffectRenderer
	{
		public string AttributeName { get; }
		public ushort ChannelAddress { get; }
		public IEnumerable<string> RenderTargets { get; }

		public DmxChannelRenderer(string attributeName, ushort channelAddress)
		{
			AttributeName = attributeName;
			ChannelAddress = channelAddress;
			RenderTargets = new[] { AttributeName };
		}

		public void Render(Dictionary<string, EffectAttribute> pipeline, Dictionary<ushort, DmxChannel> channels)
		{
			var rawValue = pipeline[AttributeName].Value;
			byte result = (byte)Scale.Rescale(rawValue, 0, 1, 0, 255);
			channels[ChannelAddress].Value = result;
		}
	}
}