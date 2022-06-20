using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Utilities;

namespace Kadmium_Dmx_Processor.Test.Utilities
{
	public class HsvTests
	{
		[Theory]
		[InlineData(0, 0, 0, 0, 0, 0)] // black
		[InlineData(0, 1, 0, 0, 0, 0)] // black
		[InlineData(0, 0, 1, 255, 255, 255)] // white
		[InlineData(180, 0, 1, 255, 255, 255)] // white
		[InlineData(0, 1, 1, 255, 0, 0)] // red
		[InlineData(120, 1, 1, 0, 255, 0)] // green
		[InlineData(240, 1, 1, 0, 0, 255)] // blue
		[InlineData(0, 0.5, 1, 255, 127, 127)] // pink
		[InlineData(0, 1, 0.5, 127, 0, 0)] // half red
		public void When_HsvToRgbIsCalled_Then_TheResultsAreAsExpected(float hue, float saturation, float brightness, byte expectedRed, byte expectedGreen, byte expectedBlue)
		{
			Hsv.HsvToRgb(hue, saturation, brightness, out byte actualRed, out byte actualGreen, out byte actualBlue);
			Assert.Equal(expectedRed, actualRed);
			Assert.Equal(expectedGreen, actualGreen);
			Assert.Equal(expectedBlue, actualBlue);
		}
	}
}