using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Services.TimeProvider;
using Kadmium_Dmx_Processor.Utilities;

namespace Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects
{
	public class FakeStrobe : IEffect
	{
		public const string InputAttributeName = "Strobe";

		public const int MAX_SPEED_HZ = 10;
		public const int MIN_SPEED_HZ = 1;

		public const float FASTEST_CYCLE_TIME_MS = 1000 / MAX_SPEED_HZ;
		public const float SLOWEST_CYCLE_TIME_MS = 1000 / MIN_SPEED_HZ;

		private ITimeProvider timeProvider;
		private TimeSpan timeSinceLastStrobe;
		private bool on = true;

		public FakeStrobe(ITimeProvider timeProvider)
		{
			this.timeProvider = timeProvider;
			this.timeSinceLastStrobe = TimeSpan.Zero;
		}

		public void Apply(Dictionary<string, EffectAttribute> pipeline)
		{
			var attribute = pipeline[InputAttributeName];
			var value = attribute.Value;
			if (value == 0)
			{
				return;
			}

			var cycleTime = Scale.Rescale(value, 0, 1, SLOWEST_CYCLE_TIME_MS, FASTEST_CYCLE_TIME_MS);
			this.timeSinceLastStrobe += timeProvider.TimeSinceLastRender;
			if (this.timeSinceLastStrobe.TotalMilliseconds > cycleTime)
			{
				this.on = !on;
				this.timeSinceLastStrobe = TimeSpan.Zero;
			}

			if (!on)
			{
				pipeline[LightFixtureConstants.Brightness].Value = 0;
			}
		}
	}
}