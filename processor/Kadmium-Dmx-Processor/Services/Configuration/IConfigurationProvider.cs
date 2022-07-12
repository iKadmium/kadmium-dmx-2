using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Services.Configuration
{
	public interface IConfigurationProvider
	{
		public string MqttHost { get; }
		public int RefreshRate { get; }
	}
}