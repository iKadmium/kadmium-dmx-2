using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.EffectRenderers;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Shared.Models;

namespace Kadmium_Dmx_Processor.Actors
{
	public class FixtureActor
	{
		public FixtureActor(FixtureInstance fixtureInstance, FixtureDefinition definition)
		{
			FixtureInstance = fixtureInstance;
			Definition = definition;
			Channels = definition.Personalities[fixtureInstance.Personality];
		}

		public FixtureInstance FixtureInstance { get; }
		public FixtureDefinition Definition { get; }
		public Dictionary<ushort, DmxChannel> Channels { get; }
		public Dictionary<string, EffectAttribute> FramePipeline { get; } = new Dictionary<string, EffectAttribute>();
		public Dictionary<string, EffectAttribute> EffectAttributes { get; } = new Dictionary<string, EffectAttribute>();
		public List<IEffectRenderer> EffectRenderers { get; } = new List<IEffectRenderer>();
		public List<IEffect> Effects { get; } = new List<IEffect>();

		public void Clear()
		{
			foreach (var attribute in EffectAttributes)
			{
				FramePipeline[attribute.Key].Value = attribute.Value.Value;
			}
		}

		public void Draw()
		{
			foreach (var effect in Effects)
			{
				effect.Apply(FramePipeline);
			}
		}

		public void Render(Memory<byte> memory)
		{
			foreach (var renderer in EffectRenderers)
			{
				renderer.Render(memory);
			}
		}

		public EffectAttribute AddAttribute(string name, bool internalOnly = false)
		{
			var framePipelineAttribute = new EffectAttribute { InternalOnly = internalOnly };
			var attribute = new EffectAttribute { InternalOnly = internalOnly };
			FramePipeline.Add(name, framePipelineAttribute);
			EffectAttributes.Add(name, attribute);
			return framePipelineAttribute;
		}
	}
}