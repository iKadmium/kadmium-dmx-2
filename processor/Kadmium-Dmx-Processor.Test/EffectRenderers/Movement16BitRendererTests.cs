using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.EffectRenderers.Movement;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Models;

namespace Kadmium_Dmx_Processor.Test.EffectRenderers
{
	public class Movement16BitRendererTests
	{
		[Theory]
		[InlineData(0, 0, 0)]
		[InlineData((0.5f), 127, 255)]
		[InlineData((127f / ushort.MaxValue), 0, 127)]
		[InlineData((256f / ushort.MaxValue), 1, 0)]
		public void When_RenderIsCalled_Then_TheOutputIsAsExpected(float value, byte expectedCoarse, byte expectedFine)
		{
			var group = "Everyone";
			var personality = "Standard";
			var definition = new FixtureDefinition(
				"Someone",
				"Something",
				new Dictionary<string, FixturePersonalityDefinition>
				{
					{
						personality,
						new FixturePersonalityDefinition(
							personality,
							new Dictionary<ushort, DmxChannel>
							{
								{ 1, new DmxChannel("TwistCoarse", 1) },
								{ 2, new DmxChannel("TwistFine", 2) }
							}
						)
					}
				},
				new Dictionary<string, Axis>
				{
					{ "Twist", new Axis("Twist", -180, 180) }
				}
			);

			var fixture = new Fixture(1, definition, personality, group);
			var renderer = new Movement16BitRenderer("Twist");

			var pipeline = new Dictionary<string, EffectAttribute>
			{
				{"Twist", new EffectAttribute() }
			};

			pipeline["Twist"].Value = value;

			renderer.Render(pipeline, fixture.Channels);

			Assert.Equal(expectedCoarse, fixture.Channels[1].Value);
			Assert.Equal(expectedFine, fixture.Channels[2].Value);
		}
	}
}