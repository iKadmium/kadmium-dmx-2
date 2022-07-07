using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.EffectRenderers;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects;
using Kadmium_Dmx_Processor.Models;
using Kadmium_Dmx_Processor.Utilities;

namespace Kadmium_Dmx_Processor.EffectRenderers.Color
{
	public class RgbDimmerRenderer : IEffectRenderer
	{
		public IEnumerable<string> RenderTargets { get; } = new[] {
			LightFixtureConstants.Red,
			LightFixtureConstants.Green,
			LightFixtureConstants.Blue,
			LightFixtureConstants.Dimmer
		};

		public void Render(Dictionary<string, EffectAttribute> pipeline, Dictionary<ushort, DmxChannel> channels)
		{
			var hue = pipeline[LightFixtureConstants.Hue].Value;
			var saturation = pipeline[LightFixtureConstants.Saturation].Value;
			var brightness = pipeline[LightFixtureConstants.Brightness].Value;

			Hsv.HsvToRgb(hue, saturation, 1, out byte red, out byte green, out byte blue);
			channels.Single(x => x.Value.Name == LightFixtureConstants.Red).Value.Value = red;
			channels.Single(x => x.Value.Name == LightFixtureConstants.Green).Value.Value = green;
			channels.Single(x => x.Value.Name == LightFixtureConstants.Blue).Value.Value = blue;

			var dimmer = (byte)Scale.Rescale(brightness, 0, 1, 0, 255);
			channels.Single(x => x.Value.Name == LightFixtureConstants.Dimmer).Value.Value = dimmer;
		}
	}
}