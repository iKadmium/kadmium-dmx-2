﻿using System.Text.Json;
using Kadmium_Dmx_Processor.Services.Configuration;
using Kadmium_Dmx_Processor.Services.EffectProvider;
using Kadmium_Dmx_Processor.Services.Groups;
using Kadmium_Dmx_Processor.Services.Mqtt;
using Kadmium_Dmx_Processor.Services.Renderer;
using Kadmium_Dmx_Processor.Services.TimeProvider;
using Kadmium_Dmx_Processor.Services.Venues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
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
					services.AddSingleton<IConfigurationProvider, EnvironmentVariableConfigurationProvider>();
					services.AddSingleton<ManagedMqttClientOptions>((serviceProvider) =>
					{
						var configProvider = serviceProvider.GetRequiredService<IConfigurationProvider>();
						return new ManagedMqttClientOptionsBuilder()
						.WithAutoReconnectDelay(TimeSpan.FromSeconds(1))
						.WithClientOptions(new MqttClientOptionsBuilder()
							.WithTcpServer(configProvider.MqttHost)
							.WithProtocolVersion(MqttProtocolVersion.V500)
							.Build()
						)
						.Build();
					});
					services.AddSingleton<IManagedMqttClient>((serviceProvider) => new MqttFactory().CreateManagedMqttClient());
					services.AddSingleton<IMqttProvider, MqttProvider>();
					services.AddSingleton<IDmxRenderer, DmxRenderer>();
					services.AddSingleton<IGroupProvider, GroupProvider>();
					services.AddSingleton<IVenueProvider, VenueProvider>();
					services.AddSingleton<IEffectProvider, EffectProvider>();
					services.AddSingleton<ITimeProvider, TimeProvider>();
					services.AddSingleton<IDmxRenderTarget, MqttRenderTarget>();
					services.AddSingleton<IMqttEventHandler, MqttEventHandler>();
				});

			builder.ConfigureLogging(loggingConfig =>
			{
				loggingConfig.AddConsole();
			});

			var host = builder.Build();

			var configProvider = host.Services.GetRequiredService<IConfigurationProvider>();

			var messageHandler = host.Services.GetRequiredService<IMqttEventHandler>();
			var mqtt = host.Services.GetRequiredService<IMqttProvider>();
			mqtt.MqttEventReceived += (sender, mqttEvent) => messageHandler.Handle(mqttEvent);
			await mqtt.Connect();

			var renderer = host.Services.GetRequiredService<IDmxRenderer>();
			renderer.Start();

			await host.RunAsync();
		}
	}
}