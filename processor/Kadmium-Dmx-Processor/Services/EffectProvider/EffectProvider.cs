using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Processor.EffectRenderers;
using Kadmium_Dmx_Processor.EffectRenderers.Color;
using Kadmium_Dmx_Processor.EffectRenderers.Movement;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects;
using Kadmium_Dmx_Processor.Effects.GroupEffects;
using Kadmium_Dmx_Processor.Models;
using Kadmium_Dmx_Processor.Services.TimeProvider;
using Kadmium_Dmx_Processor.Utilities;

namespace Kadmium_Dmx_Processor.Services.EffectProvider
{
	public class EffectProvider : IEffectProvider
	{
		private ITimeProvider TimeProvider { get; }

		public EffectProvider(ITimeProvider timeProvider)
		{
			TimeProvider = timeProvider;
		}

		public IEnumerable<IEffectRenderer> GetEffectRenderers(FixtureActor actor)
		{
			var renderers = new List<IEffectRenderer>();
			renderers.Add(GetColorRenderer(actor));

			foreach (var axis in actor.Definition.MovementAxis)
			{
				if (actor.Channels.Values.Select(x => x.Name).ContainsAll($"{axis.Key}Coarse", $"{axis.Key}Fine"))
				{
					renderers.Add(new Movement16BitRenderer(actor, axis.Value, axis.Key));
				}
			}

			// add a renderer for each remaining channel
			var occupiedChannels = renderers.SelectMany(x => x.RenderTargets);
			var remainingChannels = actor.Channels.Values.Where(x => !occupiedChannels.Select(x => x.Name).Contains(x.Name));
			foreach (var remainingChannel in remainingChannels)
			{
				renderers.Add(new DmxChannelRenderer(actor, remainingChannel));
			}

			return renderers;
		}

		private IEffectRenderer GetColorRenderer(FixtureActor actor)
		{
			var definition = actor.Definition.Personalities[actor.FixtureInstance.Personality];
			var channelNames = definition.Values.Select(x => x.Name);
			if (channelNames.ContainsAll(LightFixtureConstants.Red, LightFixtureConstants.Green, LightFixtureConstants.Blue, LightFixtureConstants.White, LightFixtureConstants.Dimmer))
			{
				return new RgbwDimmerRenderer(actor);
			}
			if (channelNames.ContainsAll(LightFixtureConstants.Red, LightFixtureConstants.Green, LightFixtureConstants.Blue, LightFixtureConstants.Dimmer))
			{
				return new RgbDimmerRenderer(actor);
			}
			if (channelNames.ContainsAll(LightFixtureConstants.Red, LightFixtureConstants.Green, LightFixtureConstants.Blue, LightFixtureConstants.White))
			{
				return new RgbwRenderer(actor);
			}
			if (channelNames.ContainsAll(LightFixtureConstants.Red, LightFixtureConstants.Green, LightFixtureConstants.Blue))
			{
				return new RgbRenderer(actor);
			}
			else
			{
				throw new ArgumentException("Definition contains no colours");
			}
		}

		public IEnumerable<IEffect> GetEffects(FixtureActor actor)
		{
			var renderers = new List<IEffect>();
			if (!actor.Definition.Personalities[actor.FixtureInstance.Personality].Values.Any(x => x.Name == LightFixtureConstants.Shutter))
			{
				renderers.Add(new FakeStrobe(TimeProvider, actor));
			}
			renderers.Add(new ApeshitClient(actor));

			return renderers;
		}

		public IEnumerable<IEffect> GetEffects(Group group)
		{
			var renderers = new List<IEffect>();
			renderers.Add(new Apeshit(TimeProvider, group));
			return renderers;
		}
	}
}