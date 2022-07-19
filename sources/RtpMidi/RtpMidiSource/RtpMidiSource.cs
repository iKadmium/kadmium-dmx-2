using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KadmiumRtpMidi;
using KadmiumRtpMidi.Packets.MidiCommands;
using Microsoft.Extensions.Logging;
using RtpMidiSource.Services;
using RtpMidiSource.Services.Mapping;
using RtpMidiSource.Services.Mqtt;
using RtpMidiSource.Services.RtpMidi;

namespace RtpMidiSource
{
	public class RtpMidiSource
	{
		private IAttributeMapProvider AttributeMapProvider { get; }
		private IGroupMapProvider GroupMapProvider { get; }
		private ISessionProvider SessionProvider { get; }
		private IAttributeSetter AttributeSetter { get; }
		private int PacketsSinceLastRender { get; set; } = 0;
		private ILogger<RtpMidiSource> Logger { get; }
		private Timer FeedbackTimer { get; }
		private Session? Session { get; set; }

		public RtpMidiSource(IAttributeMapProvider attributeMapProvider, IGroupMapProvider groupMapProvider, ISessionProvider sessionProvider, IAttributeSetter attributeSetter, ILogger<RtpMidiSource> logger)
		{
			AttributeMapProvider = attributeMapProvider;
			GroupMapProvider = groupMapProvider;
			SessionProvider = sessionProvider;
			AttributeSetter = attributeSetter;
			Logger = logger;

			FeedbackTimer = new Timer((state) =>
			{
				Logger.LogInformation($"Received {PacketsSinceLastRender} in the last second");
				PacketsSinceLastRender = 0;
			}, null, 1000, 1000);
		}

		public async Task Listen(CancellationToken token)
		{
			var attributeMap = await AttributeMapProvider.GetAttributeMapAsync();
			var groupMap = await GroupMapProvider.GetGroupMapAsync();
			Session = await SessionProvider.GetSessionAsync();

			Session.OnPacketReceived += async (_, receivedEvent) =>
			{
				var tasks = new List<Task>();

				PacketsSinceLastRender++;

				foreach (var command in receivedEvent.Packet.Commands)
				{
					if (command is ControlChange cc)
					{
						if (attributeMap.ContainsKey(cc.CcNumber) && groupMap.ContainsKey(cc.Channel))
						{
							var attribute = attributeMap[cc.CcNumber];
							var group = groupMap[cc.Channel];
							var value = cc.Value;

							var adjustedValue = attribute.GetAdjustedValue(cc.Value);

							AttributeSetter.SetAttributeCached(group, attribute.Name, adjustedValue);
						}
					}
				}

				await Task.WhenAll(tasks);
			};
		}
	}
}