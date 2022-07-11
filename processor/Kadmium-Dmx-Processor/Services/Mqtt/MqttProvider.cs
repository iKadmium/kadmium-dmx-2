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
	public class MqttProvider : IMqttProvider
	{
		public event EventHandler<MqttEvent>? MqttEventReceived;
		private IMqttClient Client { get; }
		private IMqttClientOptions ClientOptions { get; }

		public MqttProvider(IMqttClient client, IMqttClientOptions options)
		{
			Client = client;
			ClientOptions = options;
		}

		public async Task Connect()
		{
			await Client.ConnectAsync(ClientOptions);
			await Client.SubscribeAsync("/group/#");
			await Client.SubscribeAsync("/fixture/#");
			Client.UseApplicationMessageReceivedHandler(async (mqttEvent) =>
			{
				var topic = mqttEvent.ApplicationMessage.Topic;
				var payload = mqttEvent.ApplicationMessage.Payload;

				var receiverEvent = new MqttEvent(topic, payload);
				MqttEventReceived?.Invoke(this, receiverEvent);
				await mqttEvent.AcknowledgeAsync(CancellationToken.None);
			});
		}

		public async Task Send(string topic, Memory<byte> packet)
		{
			var message = new MqttApplicationMessage
			{
				Topic = topic,
				Payload = packet.ToArray()
			};
			await Client.PublishAsync(message);
		}
	}
}