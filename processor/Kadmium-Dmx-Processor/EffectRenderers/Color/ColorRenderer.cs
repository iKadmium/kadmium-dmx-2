using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects;
using Kadmium_Dmx_Shared.Models;

namespace Kadmium_Dmx_Processor.EffectRenderers.Color
{
	public abstract class ColorRenderer
	{
		protected EffectAttribute Hue { get; }
		protected EffectAttribute Saturation { get; }
		protected EffectAttribute Brightness { get; }
		protected ushort RedAddress { get; }
		protected ushort GreenAddress { get; }
		protected ushort BlueAddress { get; }
		protected List<DmxChannel> RenderTargetList { get; } = new List<DmxChannel>();
		public IEnumerable<DmxChannel> RenderTargets => RenderTargetList;

		protected ColorRenderer(FixtureActor actor, ushort fixtureAddress)
		{
			Hue = actor.AddAttribute(LightFixtureConstants.Hue);
			Saturation = actor.AddAttribute(LightFixtureConstants.Saturation);
			Brightness = actor.AddAttribute(LightFixtureConstants.Brightness);

			RedAddress = AddRenderTarget(LightFixtureConstants.Red, actor, fixtureAddress);
			GreenAddress = AddRenderTarget(LightFixtureConstants.Green, actor, fixtureAddress);
			BlueAddress = AddRenderTarget(LightFixtureConstants.Blue, actor, fixtureAddress);
		}

		protected ushort AddRenderTarget(string name, FixtureActor actor, ushort fixtureAddress)
		{
			var channel = actor.Channels.Values.Single(x => x.Name == name);
			RenderTargetList.Add(channel);
			return (ushort)(actor.Channels.Single(x => x.Value == channel).Key + fixtureAddress - 1);
		}
	}
}