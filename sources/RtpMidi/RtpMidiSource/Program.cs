using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RtpMidiSource.Services;
using RtpMidiSource.Services.Mapping;
using RtpMidiSource.Services.Mqtt;
using RtpMidiSource.Services.RtpMidi;

namespace RtpMidiSource
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			var builder = new HostBuilder()
				.ConfigureAppConfiguration((configure) => { })
				.ConfigureServices((hostContext, services) =>
				{
					services.AddSingleton<IOptionsProvider, OptionsProvider>();
					services.AddSingleton<IAttributeMapProvider, AttributeMapProvider>();
					services.AddSingleton<IGroupMapProvider, GroupMapProvider>();
					services.AddSingleton<ISessionProvider, SessionProvider>();
					services.AddSingleton<IMqttSender, MqttConnectionProvider>();
					services.AddSingleton<IAttributeSetter, MqttAttributeSetter>();
					services.AddSingleton<RtpMidiSource>();
				});
			builder.ConfigureLogging(loggingBuilder =>
			{
				loggingBuilder.AddConsole();
			});

			var host = builder.Build();

			var source = host.Services.GetRequiredService<RtpMidiSource>();
			await source.Listen(CancellationToken.None);

			await host.RunAsync();
		}
	}
}