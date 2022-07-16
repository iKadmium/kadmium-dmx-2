using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Effects;

namespace Kadmium_Dmx_Processor.Actors
{
	public class Group
	{
		public Dictionary<FixtureLocator, FixtureActor> Fixtures { get; } = new Dictionary<FixtureLocator, FixtureActor>();
		public Dictionary<string, EffectAttribute> FramePipeline { get; } = new Dictionary<string, EffectAttribute>();
		public Dictionary<string, EffectAttribute> EffectAttributes { get; } = new Dictionary<string, EffectAttribute>();
		public List<IEffect> Effects { get; } = new List<IEffect>();


		public void Clear()
		{
			foreach (var attribute in EffectAttributes)
			{
				FramePipeline[attribute.Key] = attribute.Value;
			}
		}

		public void Draw()
		{
			foreach (var effect in Effects)
			{
				effect.Apply(FramePipeline);
			}
		}

		internal EffectAttribute AddAttribute(string name)
		{
			var framePipelineAttribute = new EffectAttribute();
			var attribute = new EffectAttribute();
			FramePipeline.Add(name, framePipelineAttribute);
			EffectAttributes.Add(name, attribute);
			return framePipelineAttribute;
		}
	}

	public class FixtureLocator
	{
		public FixtureLocator(ushort universeId, ushort address)
		{
			UniverseId = universeId;
			Address = address;
		}

		public ushort UniverseId { get; set; }
		public ushort Address { get; set; }
	}
}