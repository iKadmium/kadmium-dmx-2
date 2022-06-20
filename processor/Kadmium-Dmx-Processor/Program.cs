using Kadmium_Dmx_Processor.Services.Fixtures;
using Kadmium_Dmx_Processor.Services.Groups;
using Kadmium_Dmx_Processor.Services.Mqtt;
using Kadmium_Dmx_Processor.Services.Renderer;
using Kadmium_Dmx_Processor.Services.Venues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Formatter;

namespace Kadmium_Dmx_Processor
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = new HostBuilder()
				.ConfigureAppConfiguration((configure) =>
				{
				})
				.ConfigureServices((hostContext, services) =>
				{
					services.AddSingleton<IMqttClientOptions>((serviceProvider) => new MqttClientOptionsBuilder()
						.WithTcpServer("mqtt")
						.WithProtocolVersion(MqttProtocolVersion.V500)
						.Build()
					);
					services.AddSingleton<IMqttClient>((serviceProvider) => new MqttFactory().CreateMqttClient());
					services.AddSingleton<IMqttReceiver, MqttReceiver>();
					services.AddSingleton<IDmxRenderer, DmxRenderer>();
					services.AddSingleton<IGroupProvider, GroupProvider>();
					services.AddSingleton<IFixtureDefinitionProvider, FixtureDefinitionProvider>();
					services.AddSingleton<IVenueProvider, VenueProvider>();
				});

			var host = builder.Build();

			var renderer = host.Services.GetRequiredService<IDmxRenderer>();
			await renderer.Load();

			var receiver = host.Services.GetRequiredService<IMqttReceiver>();
			receiver.MqttEventReceived += (sender, mqttEvent) =>
			{

			};
			await receiver.Connect();
			renderer.Render();

			await host.RunAsync();
		}
	}
}