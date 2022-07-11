using System.Text.Json;
using Kadmium_Dmx_Processor.Services.EffectProvider;
using Kadmium_Dmx_Processor.Services.Groups;
using Kadmium_Dmx_Processor.Services.Mqtt;
using Kadmium_Dmx_Processor.Services.Renderer;
using Kadmium_Dmx_Processor.Services.TimeProvider;
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
				.ConfigureAppConfiguration((configure) => { })
				.ConfigureServices((hostContext, services) =>
				{
					services.AddSingleton<IMqttClientOptions>((serviceProvider) => new MqttClientOptionsBuilder()
						.WithTcpServer("mqtt")
						.WithProtocolVersion(MqttProtocolVersion.V500)
						.Build()
					);
					services.AddSingleton<IMqttClient>((serviceProvider) => new MqttFactory().CreateMqttClient());
					services.AddSingleton<IMqttProvider, MqttProvider>();
					services.AddSingleton<IDmxRenderer, DmxRenderer>();
					services.AddSingleton<IGroupProvider, GroupProvider>();
					services.AddSingleton<IVenueProvider, VenueProvider>();
					services.AddSingleton<IEffectProvider, EffectProvider>();
					services.AddSingleton<ITimeProvider, TimeProvider>();
					services.AddSingleton<IDmxRenderTarget, MqttRenderTarget>();
					services.AddSingleton<IMqttEventHandler, MqttEventHandler>();
				});

			var host = builder.Build();

			var groupProvider = host.Services.GetRequiredService<IGroupProvider>();
			await groupProvider.LoadGroups();

			var venueProvider = host.Services.GetRequiredService<IVenueProvider>();

			var venueText = await File.ReadAllTextAsync("data/testVenue.json");
			var venueDoc = JsonDocument.Parse(venueText);
			venueProvider.LoadVenue(venueDoc);

			var renderer = host.Services.GetRequiredService<IDmxRenderer>();

			var messageHandler = host.Services.GetRequiredService<IMqttEventHandler>();

			var receiver = host.Services.GetRequiredService<IMqttProvider>();
			receiver.MqttEventReceived += (sender, mqttEvent) => messageHandler.Handle(mqttEvent);

			await receiver.Connect();
			var renderTimer = new Timer(async (state) =>
			{
				await renderer.Render();
			}, null, 0, 22);

			await host.RunAsync();
		}
	}
}