using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Effects
{
	public interface IEffect
	{
		void Apply(Dictionary<string, EffectAttribute> pipeline);
	}
}