using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Shared.Models;

namespace Kadmium_Dmx_Processor.EffectRenderers
{
	public interface IEffectRenderer
	{
		void Render(Memory<byte> dmxMemory);
		IEnumerable<DmxChannel> RenderTargets { get; }
	}
}