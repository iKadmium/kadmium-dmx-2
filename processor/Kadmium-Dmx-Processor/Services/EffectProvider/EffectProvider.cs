using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Processor.EffectRenderers;
using Kadmium_Dmx_Processor.EffectRenderers.Color;
using Kadmium_Dmx_Processor.EffectRenderers.Movement;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects;
using Kadmium_Dmx_Processor.Effects.FixtureEffects.MovingFixtureEffects;
using Kadmium_Dmx_Processor.Effects.GroupEffects;
using Kadmium_Dmx_Shared.Models;
using Kadmium_Dmx_Processor.Services.TimeProvider;
using Kadmium_Dmx_Processor.Utilities;

namespace Kadmium_Dmx_Processor.Services.EffectProvider
{
	public class EffectProvider : IEffectProvider
	{
		private ITimeProvider TimeProvider { get; }
		private JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		public EffectProvider(ITimeProvider timeProvider)
		{
			TimeProvider = timeProvider;
		}

		public IEnumerable<IEffectRenderer> GetEffectRenderers(FixtureActor actor, ushort fixtureAddress)
		{
			var renderers = new List<IEffectRenderer>();
			renderers.Add(GetColorRenderer(actor, fixtureAddress));

			foreach (var axis in actor.Definition.MovementAxis)
			{
				if (actor.Channels.Values.Select(x => x.Name).ContainsAll($"{axis.Key}Coarse", $"{axis.Key}Fine"))
				{
					renderers.Add(new Movement16BitRenderer(actor, axis.Value, axis.Key, fixtureAddress));
				}
			}

			// add a renderer for each remaining channel
			var occupiedChannels = renderers.SelectMany(x => x.RenderTargets);
			var remainingChannels = actor.Channels.Values.Where(x => !occupiedChannels.Select(x => x.Name).Contains(x.Name));
			foreach (var remainingChannel in remainingChannels)
			{
				renderers.Add(new DmxChannelRenderer(actor, remainingChannel, fixtureAddress));
			}

			return renderers;
		}

		private IEffectRenderer GetColorRenderer(FixtureActor actor, ushort fixtureAddress)
		{
			var definition = actor.Definition.Personalities[actor.FixtureInstance.Personality];
			var channelNames = definition.Values.Select(x => x.Name);
			if (channelNames.ContainsAll(LightFixtureConstants.Red, LightFixtureConstants.Green, LightFixtureConstants.Blue, LightFixtureConstants.White, LightFixtureConstants.Dimmer))
			{
				return new RgbwDimmerRenderer(actor, fixtureAddress);
			}
			if (channelNames.ContainsAll(LightFixtureConstants.Red, LightFixtureConstants.Green, LightFixtureConstants.Blue, LightFixtureConstants.Dimmer))
			{
				return new RgbDimmerRenderer(actor, fixtureAddress);
			}
			if (channelNames.ContainsAll(LightFixtureConstants.Red, LightFixtureConstants.Green, LightFixtureConstants.Blue, LightFixtureConstants.White))
			{
				return new RgbwRenderer(actor, fixtureAddress);
			}
			if (channelNames.ContainsAll(LightFixtureConstants.Red, LightFixtureConstants.Green, LightFixtureConstants.Blue))
			{
				return new RgbRenderer(actor, fixtureAddress);
			}
			else
			{
				throw new ArgumentException("Definition contains no colours");
			}
		}

		public IEnumerable<IEffect> GetEffects(FixtureActor actor)
		{
			var effects = new List<IEffect>();
			if (!actor.Definition.Personalities[actor.FixtureInstance.Personality].Values.Any(x => x.Name == LightFixtureConstants.Shutter))
			{
				effects.Add(new FakeStrobe(TimeProvider, actor));
			}
			effects.Add(new ApeshitClient(actor));
			effects.Add(new RandomMove(TimeProvider, actor));
			if (actor.FixtureInstance.Options.ContainsKey(nameof(AxisConstrainer)))
			{
				var options = JsonSerializer.Deserialize<Dictionary<string, AxisConstrainerOptions>>((JsonElement)actor.FixtureInstance.Options[nameof(AxisConstrainer)], JsonSerializerOptions) ?? new Dictionary<string, AxisConstrainerOptions>();
				foreach (var option in options)
				{
					effects.Add(new AxisConstrainer(option.Key, option.Value.Min, option.Value.Max, actor));
				}
			}

			return effects;
		}

		public IEnumerable<IEffect> GetEffects(Group group)
		{
			var renderers = new List<IEffect>();
			renderers.Add(new Apeshit(TimeProvider, group));
			return renderers;
		}
	}
}