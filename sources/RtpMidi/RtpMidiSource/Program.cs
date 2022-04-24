using System;
using System.Net;
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
			var attributeMapProvider = new AttributeMapProvider();
			var groupMapProvider = new GroupMapProvider();
			var sessionProvider = new SessionProvider();
			var connectionProvider = new MqttConnectionProvider();
			var attributeSetter = new MqttAttributeSetter(connectionProvider);

			var source = new RtpMidiSource(attributeMapProvider, groupMapProvider, sessionProvider, attributeSetter);
			await source.Listen(CancellationToken.None);
		}
	}
}