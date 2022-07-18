using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;

namespace SacnRenderer.Services.Mqtt
{
	public class MqttProvider : IMqttProvider
	{
		public event EventHandler<MqttEvent>? MqttEventReceived;
		private IMqttClient Client { get; }
		private MqttClientOptions ClientOptions { get; }
		private ILogger<MqttProvider> Logger { get; }
		private Timer FeedbackTimer { get; }
		private int EventsSinceLastUpdate { get; set; } = 0;

		public MqttProvider(IMqttClient client, MqttClientOptions options, ILogger<MqttProvider> logger)
		{
			Client = client;
			ClientOptions = options;
			Logger = logger;
			FeedbackTimer = new Timer((state) =>
			{
				Logger.LogInformation($"Received {EventsSinceLastUpdate} updates in the last second");
				EventsSinceLastUpdate = 0;
			}, null, 1000, 1000);
		}

		public async Task Connect()
		{
			await Client.ConnectAsync(ClientOptions);
			await Client.SubscribeAsync("/universe/#");

			Client.ApplicationMessageReceivedAsync += async (mqttEvent) =>
			{
				EventsSinceLastUpdate++;
				var topic = mqttEvent.ApplicationMessage.Topic;
				var payload = mqttEvent.ApplicationMessage.Payload;

				var receiverEvent = new MqttEvent(topic, payload);
				MqttEventReceived?.Invoke(this, receiverEvent);
				await mqttEvent.AcknowledgeAsync(CancellationToken.None);
			};
		}
	}
}