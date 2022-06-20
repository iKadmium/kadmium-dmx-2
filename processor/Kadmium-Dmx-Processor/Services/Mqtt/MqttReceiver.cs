using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Formatter;

namespace Kadmium_Dmx_Processor.Services.Mqtt
{
	public class MqttReceiver : IMqttReceiver
	{
		public event EventHandler<MqttEvent>? MqttEventReceived;
		private IMqttClient Client { get; }
		private IMqttClientOptions ClientOptions { get; }

		public MqttReceiver(IMqttClient client, IMqttClientOptions options)
		{
			Client = client;
			ClientOptions = options;
		}

		public async Task Connect()
		{
			await Client.ConnectAsync(ClientOptions);
			var response = await Client.SubscribeAsync("/group/#");
			Client.UseApplicationMessageReceivedHandler(async (mqttEvent) =>
			{
				var topic = mqttEvent.ApplicationMessage.Topic;
				var payload = mqttEvent.ApplicationMessage.Payload;
				var payloadStr = System.Text.Encoding.UTF8.GetString(payload[2..]);

				var receiverEvent = new MqttEvent
				{
					Payload = payloadStr,
					Topic = topic
				};
				MqttEventReceived?.Invoke(this, receiverEvent);
				await mqttEvent.AcknowledgeAsync(CancellationToken.None);
			});
		}
	}
}