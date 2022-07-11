using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Services.Renderer
{
	public interface IDmxRenderTarget
	{
		Task Send(ushort universe, Memory<byte> memory);
	}
}