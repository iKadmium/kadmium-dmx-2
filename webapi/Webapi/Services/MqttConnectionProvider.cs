using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Extensions.ManagedClient;

namespace Webapi.Services
{
	public class MqttConnectionProvider : IMqttConnectionProvider, IDisposable
	{
		private IManagedMqttClient Client { get; }
		private ILogger<MqttConnectionProvider> Logger { get; }

		public bool IsConnected => Client.IsStarted;

		public MqttConnectionProvider(ILogger<MqttConnectionProvider> logger)
		{
			Client = new MqttFactory().CreateManagedMqttClient();
			Logger = logger;
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

			Logger.LogInformation("Connecting to MQTT broker...");
			await Client.StartAsync(options);
		}

		public async Task PublishAsync(string topic, Memory<byte> payload, bool retain)
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