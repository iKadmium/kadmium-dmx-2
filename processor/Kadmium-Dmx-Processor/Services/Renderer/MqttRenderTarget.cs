using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Services.Mqtt;
using Kadmium_Dmx_Processor.Services.Renderer;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace Kadmium_Dmx_Processor.Services.Renderer
{
	public class MqttRenderTarget : IDmxRenderTarget
	{
		private IMqttProvider MqttProvider { get; }

		public MqttRenderTarget(IMqttProvider mqttProvider)
		{
			MqttProvider = mqttProvider;
		}

		public async Task Send(ushort universe, Memory<byte> memory)
		{
			await MqttProvider.Send($"/universe/{universe}", memory);
		}
	}
}