using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Processor.Models;
using Kadmium_Dmx_Processor.Utilities;

namespace Kadmium_Dmx_Processor.Effects.FixtureEffects.MovingFixtureEffects
{
	public class AxisConstrainer : IEffect
	{
		public EffectAttribute Attribute { get; }
		public float MinValue { get; }
		public float MaxValue { get; }

		public AxisConstrainer(string name, float minDegrees, float maxDegrees, FixtureActor actor)
		{
			var axis = actor.Definition.MovementAxis[name];
			MinValue = Scale.Rescale(minDegrees, axis.MinAngle, axis.MaxAngle, 0, 1);
			MaxValue = Scale.Rescale(maxDegrees, axis.MinAngle, axis.MaxAngle, 0, 1);
			Attribute = actor.FramePipeline[name];
		}

		public void Apply(Dictionary<string, EffectAttribute> pipeline)
		{
			var value = Scale.Rescale(Attribute.Value, 0, 1, MinValue, MaxValue);
			Attribute.Value = value;
		}
	}
}