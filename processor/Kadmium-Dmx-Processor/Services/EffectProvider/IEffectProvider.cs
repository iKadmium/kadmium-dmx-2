using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Processor.EffectRenderers;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Models;

namespace Kadmium_Dmx_Processor.Services.EffectProvider
{
	public interface IEffectProvider
	{
		IEnumerable<IEffect> GetEffects(FixtureActor actor);
		IEnumerable<IEffect> GetEffects(Group group);
		IEnumerable<IEffectRenderer> GetEffectRenderers(FixtureActor actor);
	}
}