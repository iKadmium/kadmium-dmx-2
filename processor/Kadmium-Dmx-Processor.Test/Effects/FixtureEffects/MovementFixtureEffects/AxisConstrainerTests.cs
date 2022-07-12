using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Effects.FixtureEffects.MovingFixtureEffects;
using Kadmium_Dmx_Processor.Models;

namespace Kadmium_Dmx_Processor.Test.Effects.FixtureEffects.MovementFixtureEffects
{
	public class AxisConstrainerTests
	{
		[Theory]
		[InlineData(0, -90, 90, -90, 90, 0)]
		[InlineData(0, -90, 90, 0, 90, 0.5)]
		[InlineData(0.5, -90, 90, 0, 90, 0.75)]
		[InlineData(1, -90, 90, 0, 90, 1)]
		[InlineData(1, -90, 90, -90, 0, 0.5)]
		public void When_RenderIsCalled_Then_TheResultIsAsExpected(float value, float oldMinDegrees, float oldMaxDegrees, float newMinDegrees, float newMaxDegrees, float expectedValue)
		{
			string axisName = "Axis";
			var axis = new Axis(oldMinDegrees, oldMaxDegrees);
			var fixture = FixtureHelper.GetFixture(axisName);
			var attribute = fixture.AddAttribute(axisName);
			var effect = new AxisConstrainer(axisName, newMinDegrees, newMaxDegrees, fixture);
			attribute.Value = value;

			effect.Apply(fixture.FramePipeline);

			Assert.Equal(expectedValue, attribute.Value);
		}
	}
}