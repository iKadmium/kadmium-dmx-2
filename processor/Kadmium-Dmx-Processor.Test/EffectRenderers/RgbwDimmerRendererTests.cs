using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.EffectRenderers.Color;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects;
using Kadmium_Dmx_Processor.Models;

namespace Kadmium_Dmx_Processor.Test.EffectRenderers
{
	public class RgbwDimmerRendererTests
	{
		[Theory]
		[InlineData(0,      0,      0,      0,      0,      0,      255,	0)] // black
		[InlineData(0,      1,      0,      255,    0,      0,      0,		0)] // black
		[InlineData(0,      0,      1,      0,      0,      0,      255,	255)] // white
		[InlineData(180,    0,      1,      0,      0,      0,      255,	255)] // white
		[InlineData(0,      1,      1,      255,    0,      0,      0,		255)] // red
		[InlineData(120,    1,      1,      0,      255,    0,      0,		255)] // green
		[InlineData(240,    1,      1,      0,      0,      255,    0,		255)] // blue
		[InlineData(0,      0.5,    1,      127,    0,      0,      127,	255)] // pink
		[InlineData(0,      1,      0.5,    255,    0,      0,      0,		127)] // half red
        [InlineData(0,      0.5,    0.5,    127,    0,      0,      127,	127)] // half pink
		public void When_RenderIsCalled_Then_TheResultsAreAsExpected(float hue, float saturation, float brightness, byte expectedRed, byte expectedGreen, byte expectedBlue, byte expectedWhite, byte expectedDimmer)
		{
			var pipeline = new Dictionary<string, EffectAttribute>
			{
				{LightFixtureConstants.Hue, new EffectAttribute{Value = hue}},
				{LightFixtureConstants.Saturation, new EffectAttribute{Value = saturation}},
				{LightFixtureConstants.Brightness, new EffectAttribute{Value = brightness}}
			};
			var channels = new Dictionary<ushort, DmxChannel>
			{
				{1, new DmxChannel(LightFixtureConstants.Red, 1)},
				{2, new DmxChannel(LightFixtureConstants.Green, 2)},
				{3, new DmxChannel(LightFixtureConstants.Blue, 3)},
				{4, new DmxChannel(LightFixtureConstants.White, 4)},
				{5, new DmxChannel(LightFixtureConstants.Dimmer, 5)}
			};

			var renderer = new RgbwDimmerRenderer();
			renderer.Render(pipeline, channels);

			Assert.Equal(channels[1].Value, expectedRed);
			Assert.Equal(channels[2].Value, expectedGreen);
			Assert.Equal(channels[3].Value, expectedBlue);
			Assert.Equal(channels[4].Value, expectedWhite);
			Assert.Equal(channels[5].Value, expectedDimmer);
		}
	}
}