using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects;
using Kadmium_Dmx_Processor.Services.TimeProvider;
using Kadmium_Dmx_Processor.Utilities;

namespace Kadmium_Dmx_Processor.Effects.GroupEffects
{
	public class Apeshit : IEffect
	{
		public const int MAX_SPEED_HZ = 5;
		public const int MIN_SPEED_HZ = 1;

		public const float FASTEST_CYCLE_TIME_MS = 1000 / MAX_SPEED_HZ;
		public const float SLOWEST_CYCLE_TIME_MS = 1000 / MIN_SPEED_HZ;

		private ITimeProvider TimeProvider { get; }
		private TimeSpan TimeSinceLastChange { get; set; }
		private EffectAttribute ApeshitAttribute { get; }
		private Group Group { get; }
		private float LastValue { get; set; }

		public Apeshit(ITimeProvider timeProvider, Group group)
		{
			ApeshitAttribute = group.AddAttribute(LightFixtureConstants.Apeshit);
			TimeProvider = timeProvider;
			TimeSinceLastChange = TimeSpan.Zero;
			Group = group;
		}

		public void Apply(Dictionary<string, EffectAttribute> pipeline)
		{
			var value = ApeshitAttribute.Value;
			if (value == 0f)
			{
				if (LastValue != 0f)
				{
					foreach (var fixture in Group.Fixtures)
					{
						fixture.FramePipeline[LightFixtureConstants.ApeshitActive].Value = 0f;
						fixture.FramePipeline[LightFixtureConstants.ApeshitBlackout].Value = 0f;
					}
				}
				LastValue = value;
				return;
			}

			var cycleTime = Scale.Rescale(value, 0, 1, SLOWEST_CYCLE_TIME_MS, FASTEST_CYCLE_TIME_MS);
			this.TimeSinceLastChange += TimeProvider.TimeSinceLastRender;
			if (this.TimeSinceLastChange.TotalMilliseconds > cycleTime)
			{
				var onFixtures = Group.Fixtures.TakeRandom(Group.Fixtures.Count / 3);
				var offFixtures = Group.Fixtures.Except(onFixtures);
				foreach (var fixture in onFixtures)
				{
					fixture.EffectAttributes[LightFixtureConstants.ApeshitBlackout].Value = 0f;
					fixture.EffectAttributes[LightFixtureConstants.ApeshitActive].Value = value;
				}
				foreach (var fixture in offFixtures)
				{
					fixture.EffectAttributes[LightFixtureConstants.ApeshitBlackout].Value = 1f;
					fixture.EffectAttributes[LightFixtureConstants.ApeshitActive].Value = 0f;
				}
				this.TimeSinceLastChange = TimeSpan.Zero;
			}

			LastValue = value;
		}
	}
}