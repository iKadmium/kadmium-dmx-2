using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.EffectRenderers;
using Kadmium_Dmx_Processor.Models;
using Kadmium_Dmx_Processor.Utilities;

namespace Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects.Renderers
{
	public class RgbRenderer : IEffectRenderer
	{
		public void Render(Dictionary<string, EffectAttribute> pipeline, Dictionary<ushort, DmxChannel> channels)
		{
			var hue = pipeline[LightFixtureConstants.Hue].Value;
			var saturation = pipeline[LightFixtureConstants.Saturation].Value;
			var brightness = pipeline[LightFixtureConstants.Brightness].Value;

			Hsv.HsvToRgb(hue, saturation, brightness, out byte red, out byte green, out byte blue);
			channels.Single(x => x.Value.Name == LightFixtureConstants.Red).Value.Value = red;
			channels.Single(x => x.Value.Name == LightFixtureConstants.Green).Value.Value = green;
			channels.Single(x => x.Value.Name == LightFixtureConstants.Blue).Value.Value = blue;
		}
	}
}