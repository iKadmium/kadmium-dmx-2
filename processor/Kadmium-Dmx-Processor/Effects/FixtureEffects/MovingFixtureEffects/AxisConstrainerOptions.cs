using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Effects.FixtureEffects.MovingFixtureEffects
{
	public class AxisConstrainerOptions
	{
		public AxisConstrainerOptions(float min, float max)
		{
			Min = min;
			Max = max;
		}

		public float Min { get; }
		public float Max { get; }
	}
}