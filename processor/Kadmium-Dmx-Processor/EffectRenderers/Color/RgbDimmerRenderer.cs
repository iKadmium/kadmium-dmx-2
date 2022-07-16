using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Processor.EffectRenderers;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects;
using Kadmium_Dmx_Shared.Models;
using Kadmium_Dmx_Processor.Utilities;

namespace Kadmium_Dmx_Processor.EffectRenderers.Color
{
	public class RgbDimmerRenderer : ColorRenderer, IEffectRenderer
	{
		private ushort DimmerAddress { get; }

		public RgbDimmerRenderer(FixtureActor actor, ushort fixtureAddress) : base(actor, fixtureAddress)
		{
			DimmerAddress = AddRenderTarget(LightFixtureConstants.Dimmer, actor, fixtureAddress);
		}

		public void Render(Memory<byte> dmxMemory)
		{
			var hue = Hue.Value;
			var saturation = Saturation.Value;
			var brightness = Brightness.Value;

			Hsv.HsvToRgb(hue, saturation, 1, out byte red, out byte green, out byte blue);
			dmxMemory.Span[RedAddress] = red;
			dmxMemory.Span[GreenAddress] = green;
			dmxMemory.Span[BlueAddress] = blue;

			var dimmer = (byte)Scale.Rescale(brightness, 0, 1, 0, 255);
			dmxMemory.Span[DimmerAddress] = dimmer;
		}
	}
}