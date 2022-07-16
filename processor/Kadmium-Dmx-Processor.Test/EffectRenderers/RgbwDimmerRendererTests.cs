using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.EffectRenderers.Color;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects;
using Kadmium_Dmx_Shared.Models;

namespace Kadmium_Dmx_Processor.Test.EffectRenderers
{
	public class RgbwDimmerRendererTests
	{
		[Theory]
		[InlineData(0, 0, 0, 0, 0, 0, 255, 0)] // black
		[InlineData(0, 1, 0, 255, 0, 0, 0, 0)] // black
		[InlineData(0, 0, 1, 0, 0, 0, 255, 255)] // white
		[InlineData(180, 0, 1, 0, 0, 0, 255, 255)] // white
		[InlineData(0, 1, 1, 255, 0, 0, 0, 255)] // red
		[InlineData(120, 1, 1, 0, 255, 0, 0, 255)] // green
		[InlineData(240, 1, 1, 0, 0, 255, 0, 255)] // blue
		[InlineData(0, 0.5, 1, 127, 0, 0, 127, 255)] // pink
		[InlineData(0, 1, 0.5, 255, 0, 0, 0, 127)] // half red
		[InlineData(0, 0.5, 0.5, 127, 0, 0, 127, 127)] // half pink
		public void When_RenderIsCalled_Then_TheResultsAreAsExpected(float hue, float saturation, float brightness, byte expectedRed, byte expectedGreen, byte expectedBlue, byte expectedWhite, byte expectedDimmer)
		{
			var fixture = FixtureHelper.GetFixture(
				LightFixtureConstants.Red,
				LightFixtureConstants.Green,
				LightFixtureConstants.Blue,
				LightFixtureConstants.White,
				LightFixtureConstants.Dimmer
			);

			var memory = new Memory<byte>(new byte[Universe.MAX_SIZE]);
			var renderer = new RgbwDimmerRenderer(fixture, 1);

			fixture.FramePipeline[LightFixtureConstants.Hue].Value = hue;
			fixture.FramePipeline[LightFixtureConstants.Saturation].Value = saturation;
			fixture.FramePipeline[LightFixtureConstants.Brightness].Value = brightness;

			renderer.Render(memory);

			Assert.Equal(memory.Span[1], expectedRed);
			Assert.Equal(memory.Span[2], expectedGreen);
			Assert.Equal(memory.Span[3], expectedBlue);
			Assert.Equal(memory.Span[4], expectedWhite);
			Assert.Equal(memory.Span[5], expectedDimmer);
		}
	}
}