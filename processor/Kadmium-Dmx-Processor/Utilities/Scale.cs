using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Utilities
{
	public static class Scale
	{
		public static float Rescale(float value, float oldMin, float oldMax, float newMin, float newMax)
		{
			var oldRange = oldMax - oldMin;
			var rawValue = (value - oldMin) / oldRange;

			var newRange = newMax - newMin;
			var scaled = (rawValue * newRange) + newMin;

			return scaled;
		}
	}
}