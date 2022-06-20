using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Services.TimeProvider
{
	public class TimeProvider : ITimeProvider
	{
		private DateTime lastRenderTime;

		public TimeProvider()
		{
			lastRenderTime = DateTime.Now;
		}

		public TimeSpan TimeSinceLastRender => DateTime.Now - lastRenderTime;

		public void OnRender()
		{
			lastRenderTime = DateTime.Now;
		}
	}
}