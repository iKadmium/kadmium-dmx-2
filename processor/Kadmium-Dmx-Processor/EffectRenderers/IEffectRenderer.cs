using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Models;

namespace Kadmium_Dmx_Processor.EffectRenderers
{
	public interface IEffectRenderer
	{
		void Render(Dictionary<string, EffectAttribute> pipeline, Dictionary<ushort, DmxChannel> channels);
		IEnumerable<string> RenderTargets { get; }
	}
}