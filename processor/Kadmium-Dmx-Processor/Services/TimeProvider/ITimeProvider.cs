using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Services.TimeProvider
{
	public interface ITimeProvider
	{
		TimeSpan TimeSinceLastRender { get; }
		void OnRender();
	}
}