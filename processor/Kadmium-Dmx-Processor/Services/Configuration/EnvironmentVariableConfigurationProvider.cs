using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Services.Configuration
{
	public class EnvironmentVariableConfigurationProvider : IConfigurationProvider
	{
		public string MqttHost { get; } = Environment.GetEnvironmentVariable("MQTT_HOST") ?? "mqtt";
		public int RefreshRate { get; } = int.Parse(Environment.GetEnvironmentVariable("DMX_REFRESH_RATE") ?? "45");
	}
}