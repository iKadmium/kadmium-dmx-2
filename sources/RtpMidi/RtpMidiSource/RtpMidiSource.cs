using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KadmiumRtpMidi;
using KadmiumRtpMidi.Packets.MidiCommands;
using Microsoft.Extensions.Logging;
using RtpMidiSource.Services;
using RtpMidiSource.Services.Mqtt;
using RtpMidiSource.Services.RtpMidi;
using RtpMidiSource.Services.ValueCache;

namespace RtpMidiSource
{
	public class RtpMidiSource
	{
		private ISessionProvider SessionProvider { get; }
		private IValueCache ValueCache { get; }
		private int PacketsSinceLastRender { get; set; } = 0;
		private ILogger<RtpMidiSource> Logger { get; }
		private Timer FeedbackTimer { get; }
		private Session? Session { get; set; }

		public RtpMidiSource(ISessionProvider sessionProvider, IValueCache valueCache, ILogger<RtpMidiSource> logger)
		{
			SessionProvider = sessionProvider;
			ValueCache = valueCache;
			Logger = logger;

			FeedbackTimer = new Timer((state) =>
			{
				Logger.LogInformation($"Received {PacketsSinceLastRender} in the last second");
				PacketsSinceLastRender = 0;
			}, null, 1000, 1000);
		}

		public async Task Listen(CancellationToken token)
		{
			Session = await SessionProvider.GetSessionAsync();

			Session.OnPacketReceived += async (_, receivedEvent) =>
			{
				var tasks = new List<Task>();

				PacketsSinceLastRender++;

				foreach (var command in receivedEvent.Packet.Commands)
				{
					if (command is ControlChange cc)
					{
						ValueCache.SetValue(cc.Channel, cc.CcNumber, cc.Value);
					}
				}

				await Task.WhenAll(tasks);
			};
		}
	}
}