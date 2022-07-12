using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MQTTnet.Client;

namespace SacnRenderer.Services.Mqtt
{
	public class MqttProvider : IMqttProvider
	{
		public event EventHandler<MqttEvent>? MqttEventReceived;
		private IMqttClient Client { get; }
		private MqttClientOptions ClientOptions { get; }

		public MqttProvider(IMqttClient client, MqttClientOptions options)
		{
			Client = client;
			ClientOptions = options;
		}

		public async Task Connect()
		{
			await Client.ConnectAsync(ClientOptions);
			await Client.SubscribeAsync("/universe/#");

			Client.ApplicationMessageReceivedAsync += async (mqttEvent) =>
			{
				var topic = mqttEvent.ApplicationMessage.Topic;
				var payload = mqttEvent.ApplicationMessage.Payload;

				var receiverEvent = new MqttEvent(topic, payload);
				MqttEventReceived?.Invoke(this, receiverEvent);
				await mqttEvent.AcknowledgeAsync(CancellationToken.None);
			};
		}
	}
}