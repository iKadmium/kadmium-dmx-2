using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Processor.Services.TimeProvider;
using Kadmium_Dmx_Processor.Utilities;

namespace Kadmium_Dmx_Processor.Effects.FixtureEffects.MovingFixtureEffects
{
	public class RandomMove : IEffect
	{
		public const float FASTEST_PATH_TIME_MS = 2000;
		public const float SLOWEST_PATH_TIME_MS = 5000;

		public const float FASTEST_PATH_SPEED = 1000 / FASTEST_PATH_TIME_MS;
		public const float SLOWEST_PATH_SPEED = 1000 / SLOWEST_PATH_TIME_MS;

		public Dictionary<string, EffectAttribute> LocationAttributes { get; }
		public EffectAttribute RandomMoveAttribute { get; }
		private ITimeProvider TimeProvider { get; }
		private float PathCompletion { get; set; } = 0f;
		private float PathSpeed { get; set; }
		private Dictionary<string, float> TargetLocations { get; set; }
		private Dictionary<string, float> SourceLocations { get; set; }
		private float LastValue { get; set; } = 0f;

		public RandomMove(ITimeProvider timeProvider, FixtureActor actor)
		{
			TimeProvider = timeProvider;
			LocationAttributes = actor.Definition.MovementAxis.ToDictionary(x => x.Key, x => actor.FramePipeline[x.Key]);
			SourceLocations = actor.Definition.MovementAxis.ToDictionary(x => x.Key, x => 0f);
			TargetLocations = actor.Definition.MovementAxis.ToDictionary(x => x.Key, x => 0f);
			RandomMoveAttribute = actor.AddAttribute("RandomMove");
		}

		public void Apply(Dictionary<string, EffectAttribute> pipeline)
		{
			var value = RandomMoveAttribute.Value;
			if (value == 0f)
			{
				LastValue = value;
				return;
			}

			if (value != LastValue || PathCompletion >= 1f)
			{
				PickNewTarget(value);
			}

			PathCompletion += (float)(PathSpeed * TimeProvider.TimeSinceLastRender.TotalSeconds);
			if (PathCompletion > 1f)
			{
				PathCompletion = 1f;
			}
			var eased = EasingFunctions.InOutQuad(PathCompletion);
			foreach (var axis in SourceLocations.Keys)
			{
				LocationAttributes[axis].Value = Scale.Rescale(eased, 0, 1, SourceLocations[axis], TargetLocations[axis]);
			}
		}

		private void PickNewTarget(float speedFactor)
		{
			float distance = 0;
			foreach (var axis in SourceLocations.Keys)
			{
				SourceLocations[axis] = LocationAttributes[axis].Value;
				TargetLocations[axis] = Random.Shared.NextSingle();
				distance += Math.Abs(TargetLocations[axis] - SourceLocations[axis]);
			}

			PathCompletion = 0f;
			var speed = Scale.Rescale(speedFactor, 0, 1, SLOWEST_PATH_SPEED, FASTEST_PATH_SPEED);
			PathSpeed = speed / distance;
		}
	}
}