using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;

namespace Kadmium_Dmx_Processor.Services.Mqtt
{
	public class MqttProvider : IMqttProvider
	{
		public event EventHandler<MqttEvent>? MqttEventReceived;
		private IMqttClient Client { get; }
		private MqttClientOptions ClientOptions { get; }
		private List<string> Subscriptions { get; } = new List<string>();

		public MqttProvider(IMqttClient client, MqttClientOptions options)
		{
			Client = client;
			ClientOptions = options;
		}

		public async Task Connect()
		{
			await Client.ConnectAsync(ClientOptions);
			await Client.SubscribeAsync("/venue/load");

			Client.ApplicationMessageReceivedAsync += async (mqttEvent) =>
			{
				var topic = mqttEvent.ApplicationMessage.Topic;
				var payload = mqttEvent.ApplicationMessage.Payload;

				var receiverEvent = new MqttEvent(topic, payload);
				MqttEventReceived?.Invoke(this, receiverEvent);
				await mqttEvent.AcknowledgeAsync(CancellationToken.None);
			};
		}

		public async Task Send(string topic, Memory<byte> packet, bool retain = true)
		{
			var message = new MqttApplicationMessage
			{
				Topic = topic,
				Payload = packet.ToArray(),
				Retain = retain
			};
			await Client.PublishAsync(message);
		}

		public Task Subscribe(params string[] topics)
		{
			return Subscribe(topics);
		}

		public Task UnsubscribeAll()
		{
			var tasks = Subscriptions.Select(x => Client.SubscribeAsync(x));
			Subscriptions.Clear();
			return Task.WhenAll(tasks);
		}

		public Task Subscribe(IEnumerable<string> topics)
		{
			Subscriptions.AddRange(topics);
			var tasks = topics.Select(x => Client.SubscribeAsync(x));
			return Task.WhenAll(tasks);
		}
	}
}