using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Formatter;

namespace RtpMidiSource.Services.Mqtt
{
	public class MqttConnectionProvider : IMqttSender, IDisposable
	{
		private IMqttClient Client { get; }
		private Task? connectTask = null;

		public MqttConnectionProvider()
		{
			Client = new MqttFactory().CreateMqttClient();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		private async Task Connect()
		{
			var options = new MqttClientOptionsBuilder()
							.WithTcpServer("mqtt")
							.WithProtocolVersion(MqttProtocolVersion.V500)
							.Build();

			Console.WriteLine("Conencting to MQTT broker...");

			await Client.ConnectAsync(options);
		}

		public async Task PublishAsync(string topic, Memory<byte> payload)
		{
			if (!Client.IsConnected)
			{
				if (connectTask == null)
				{
					connectTask = Connect();
				}
				await connectTask;
			}

			var message = new MqttApplicationMessageBuilder()
				.WithTopic(topic)
				.WithPayload(payload.ToArray())
				.Build();

			await Client.PublishAsync(message, CancellationToken.None);
		}
	}
}