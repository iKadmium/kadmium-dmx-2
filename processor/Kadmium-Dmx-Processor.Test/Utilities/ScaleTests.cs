using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Utilities;

namespace Kadmium_Dmx_Processor.Test.Utilities
{
	public class ScaleTests
	{
		[Theory]
		[InlineData(0, 0, 1, 0, 255, 0)] // byte 0.0 -> 0
		[InlineData(1, 0, 1, 0, 255, 255)] // byte 1.0 -> 255
		[InlineData(0, -10, 10, 0, 1, 0.5)] // 0, -10 to 10, 0 to 1 -> 0.5
		[InlineData(0.5, 0, 1, -50, 50, 0)] // 0.5, 0 to 1, -50 to 50 -> 0
		[InlineData(-20, -20, -40, 20, 40, 20)] // -20, -20 to -40, 20 to 40 -> 20
		public void When_RescaleIsCalled_Then_TheOutputIsCorrect(float value, float oldMin, float oldMax, float newMin, float newMax, float expected)
		{
			var actual = Scale.Rescale(value, oldMin, oldMax, newMin, newMax);
			Assert.Equal(expected, actual);
		}
	}
}