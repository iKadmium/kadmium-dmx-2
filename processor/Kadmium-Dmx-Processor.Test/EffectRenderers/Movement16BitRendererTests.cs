using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
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
			var personality = "Standard";
			var definition = new FixtureDefinition(
				"Someone",
				"Something",
				new Dictionary<string, Dictionary<ushort, DmxChannel>>
				{
					{
						personality,
						new Dictionary<ushort, DmxChannel>
						{
							{ 1, new DmxChannel("TwistCoarse") },
							{ 2, new DmxChannel("TwistFine") }
						}
					}
				},
				new Dictionary<string, Axis>
				{
					{ "Twist", new Axis(-180, 180) }
				}
			);

			var fixtureInstance = new FixtureInstance(1, "Someone", "Something", personality);
			var actor = new FixtureActor(fixtureInstance, definition);

			var renderer = new Movement16BitRenderer(actor, definition.MovementAxis["Twist"], "Twist");

			actor.FramePipeline["Twist"].Value = value;

			var memory = new Memory<byte>(new byte[Universe.MAX_SIZE]);

			renderer.Render(memory);

			Assert.Equal(memory.Span[1], expectedCoarse);
			Assert.Equal(memory.Span[2], expectedFine);
		}
	}
}