using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;

namespace Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects
{
	public class ApeshitClient : IEffect
	{
		public FixtureActor Actor { get; }
		public EffectAttribute BlackoutAttribute { get; }
		public EffectAttribute ActiveAttribute { get; }
		public EffectAttribute BrightnessAttribute { get; }
		public EffectAttribute ShutterAttribute { get; }

		public ApeshitClient(FixtureActor actor)
		{
			Actor = actor;
			BlackoutAttribute = actor.AddAttribute(LightFixtureConstants.ApeshitBlackout);
			ActiveAttribute = actor.AddAttribute(LightFixtureConstants.ApeshitActive);
			BrightnessAttribute = actor.FramePipeline[LightFixtureConstants.Brightness];
			ShutterAttribute = actor.FramePipeline[LightFixtureConstants.Shutter];
		}

		public void Apply(Dictionary<string, EffectAttribute> pipeline)
		{
			if (BlackoutAttribute.Value == 1f)
			{
				BrightnessAttribute.Value = 0f;
			}
			else if (ActiveAttribute.Value > 0)
			{
				ShutterAttribute.Value = ActiveAttribute.Value;
			}
		}
	}
}