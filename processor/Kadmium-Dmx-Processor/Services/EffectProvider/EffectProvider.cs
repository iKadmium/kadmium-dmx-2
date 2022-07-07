using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.EffectRenderers;
using Kadmium_Dmx_Processor.EffectRenderers.Color;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects;
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

		public IEnumerable<IEffectRenderer> GetEffectRenderers(FixtureDefinition fixtureDefinition, string personality)
		{
			var renderers = new List<IEffectRenderer>();
			var personalityDefinition = fixtureDefinition.Personalities[personality];
			renderers.Add(GetColorRenderer(personalityDefinition));

			// add a renderer for each remaining channel
			var occupiedChannels = renderers.SelectMany(x => x.RenderTargets);
			var remainingChannels = personalityDefinition.Channels.Values.Where(x => !occupiedChannels.Contains(x.Name));
			foreach (var remainingChannel in remainingChannels)
			{
				renderers.Add(new DmxChannelRenderer(remainingChannel.Name, remainingChannel.Address));
			}

			return renderers;
		}

		private IEffectRenderer GetColorRenderer(FixturePersonalityDefinition definition)
		{
			var channelNames = definition.Channels.Select(x => x.Value.Name);
			if (channelNames.ContainsAll(LightFixtureConstants.Red, LightFixtureConstants.Green, LightFixtureConstants.Blue, LightFixtureConstants.White, LightFixtureConstants.Dimmer))
			{
				return new RgbwDimmerRenderer();
			}
			if (channelNames.ContainsAll(LightFixtureConstants.Red, LightFixtureConstants.Green, LightFixtureConstants.Blue, LightFixtureConstants.Dimmer))
			{
				return new RgbDimmerRenderer();
			}
			if (channelNames.ContainsAll(LightFixtureConstants.Red, LightFixtureConstants.Green, LightFixtureConstants.Blue, LightFixtureConstants.White))
			{
				return new RgbwRenderer();
			}
			if (channelNames.ContainsAll(LightFixtureConstants.Red, LightFixtureConstants.Green, LightFixtureConstants.Blue))
			{
				return new RgbRenderer();
			}
			else
			{
				throw new ArgumentException("Definition contains no colours");
			}
		}

		public IEnumerable<IEffect> GetEffects(FixtureDefinition fixtureDefinition, string personality)
		{
			var renderers = new List<IEffect>();
			var personalityDefinition = fixtureDefinition.Personalities[personality];
			if (!personalityDefinition.Channels.Values.Any(x => x.Name == LightFixtureConstants.Shutter))
			{
				renderers.Add(new FakeStrobe(TimeProvider));
			}

			return renderers;
		}
	}
}