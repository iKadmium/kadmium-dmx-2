using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Extensions.ManagedClient;

namespace RtpMidiSource.Services.Mqtt
{
	public class MqttConnectionProvider : IMqttSender, IDisposable
	{
		private IManagedMqttClient Client { get; }
		private Dictionary<string, Func<MqttApplicationMessageReceivedEventArgs, Task>> Subscriptions { get; } = new();

		public bool IsConnected => Client.IsStarted;

		public MqttConnectionProvider()
		{
			Client = new MqttFactory().CreateManagedMqttClient();
		}

		public void Dispose()
		{
			Client.Dispose();
		}

		public async Task Begin()
		{
			var options = new ManagedMqttClientOptionsBuilder()
							.WithClientOptions(new MqttClientOptionsBuilder()
								.WithTcpServer("mqtt")
								.WithProtocolVersion(MqttProtocolVersion.V500)
								.Build()
							).Build();

			Console.WriteLine("Connecting to MQTT broker...");
			await Client.StartAsync(options);
		}

		public async Task PublishAsync(string topic, Memory<byte> payload)
		{
			var message = new MqttApplicationMessageBuilder()
				.WithTopic(topic)
				.WithPayload(payload.ToArray())
				.WithRetainFlag(true)
				.Build();

			await Client.EnqueueAsync(message);
		}

		public async Task SubscribeAsync(string topic, Func<MqttApplicationMessageReceivedEventArgs, Task> handler)
		{
			Subscriptions.Add(topic, handler);
			await Client.SubscribeAsync(topic);
			Client.ApplicationMessageReceivedAsync += handler;
		}
	}
}