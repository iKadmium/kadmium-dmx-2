using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Processor.EffectRenderers;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects;
using Kadmium_Dmx_Processor.Models;
using Kadmium_Dmx_Processor.Utilities;

namespace Kadmium_Dmx_Processor.EffectRenderers.Color
{
	public class RgbRenderer : ColorRenderer, IEffectRenderer
	{
		public RgbRenderer(FixtureActor actor) : base(actor) { }

		public void Render(Memory<byte> dmxMemory)
		{
			var hue = Hue.Value;
			var saturation = Saturation.Value;
			var brightness = Brightness.Value;

			Hsv.HsvToRgb(hue, saturation, brightness, out byte red, out byte green, out byte blue);
			dmxMemory.Span[RedAddress] = red;
			dmxMemory.Span[GreenAddress] = green;
			dmxMemory.Span[BlueAddress] = blue;
		}
	}
}