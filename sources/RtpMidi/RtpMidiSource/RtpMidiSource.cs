using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KadmiumRtpMidi.Packets.MidiCommands;
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

		public RtpMidiSource(IAttributeMapProvider attributeMapProvider, IGroupMapProvider groupMapProvider, ISessionProvider sessionProvider, IAttributeSetter attributeSetter)
		{
			AttributeMapProvider = attributeMapProvider;
			GroupMapProvider = groupMapProvider;
			SessionProvider = sessionProvider;
			AttributeSetter = attributeSetter;
		}

		public async Task Listen(CancellationToken token)
		{
			var attributeMap = await AttributeMapProvider.GetAttributeMapAsync();
			var groupMap = await GroupMapProvider.GetGroupMapAsync();
			var session = await SessionProvider.GetSessionAsync();

			session.OnPacketReceived += async (_, receivedEvent) =>
			{
				var tasks = new List<Task>();

				foreach (var command in receivedEvent.Packet.Commands)
				{
					if (command is ControlChange cc)
					{
						if (attributeMap.ContainsKey(cc.CcNumber) && groupMap.ContainsKey(cc.Channel))
						{
							var attribute = attributeMap[cc.CcNumber];
							var group = groupMap[cc.Channel];
							var value = cc.Value;

							tasks.Add(AttributeSetter.SetAttributeAsync(group, attribute, value));
						}
					}
				}

				await Task.WhenAll(tasks);
			};

			while (!token.IsCancellationRequested)
			{
				await Task.Delay(500, token);
			}
		}
	}
}