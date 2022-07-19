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

		public bool IsConnected => Client.IsStarted;

		public MqttConnectionProvider()
		{
			Client = new MqttFactory().CreateManagedMqttClient();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
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
	}
}