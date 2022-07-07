using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.EffectRenderers;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Models;

namespace Kadmium_Dmx_Processor.Services.EffectProvider
{
	public interface IEffectProvider
	{
		IEnumerable<IEffect> GetEffects(FixtureDefinition fixtureDefinition, string personality);
		IEnumerable<IEffectRenderer> GetEffectRenderers(FixtureDefinition fixtureDefinition, string personality);
	}
}