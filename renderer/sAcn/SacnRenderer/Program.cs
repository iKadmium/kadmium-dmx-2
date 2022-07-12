using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using SacnRenderer.Services.Mqtt;
using SacnRenderer.Services.Sacn;

namespace SacnRenderer
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = new HostBuilder()
				.ConfigureAppConfiguration((configure) => { })
				.ConfigureServices((hostContext, services) =>
				{
					services.AddSingleton<MqttClientOptions>((serviceProvider) => new MqttClientOptionsBuilder()
						.WithTcpServer("mqtt")
						.WithProtocolVersion(MqttProtocolVersion.V500)
						.Build()
					);

					services.AddSingleton<IMqttClient>((serviceProvider) => new MqttFactory().CreateMqttClient());
					services.AddSingleton<IMqttProvider, MqttProvider>();
					services.AddSingleton<ISacnRenderer, SacnRenderer.Services.Sacn.SacnRenderer>();
				});

			var host = builder.Build();

			var listener = host.Services.GetRequiredService<IMqttProvider>();
			await listener.Connect();

			var renderer = host.Services.GetRequiredService<ISacnRenderer>();
			listener.MqttEventReceived += async (sender, mqttEvent) =>
			{
				var parts = mqttEvent.Topic.Split("/", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				var universeIdPart = parts.Last();
				var universeId = ushort.Parse(universeIdPart);
				await renderer.Render(universeId, mqttEvent.Payload);
			};

			await host.RunAsync();
		}
	}
}