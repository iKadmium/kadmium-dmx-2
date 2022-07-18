using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Services.Renderer
{
	public interface IDmxRenderer
	{
		Task Render();
		void Start();
	}
}