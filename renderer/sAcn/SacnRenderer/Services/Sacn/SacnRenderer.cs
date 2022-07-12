using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_sACN;
using Kadmium_sACN.SacnSender;
using SacnRenderer.Services.Configuration;

namespace SacnRenderer.Services.Sacn
{
	public class SacnRenderer : ISacnRenderer
	{
		private SacnPacketFactory PacketFactory { get; }
		private IEnumerable<MulticastSacnSender> MulticastSenders { get; }
		private IEnumerable<UnicastSacnSender> UnicastSenders { get; }

		public SacnRenderer(IConfigurationProvider configurationProvider)
		{
			PacketFactory = new Kadmium_sACN.SacnPacketFactory(new byte[] { 1, 2, 3, 4 }, "Kadmium-DMX");
			var multicastSenders = new List<MulticastSacnSender>();
			if (configurationProvider.Ipv4Multicast)
			{
				multicastSenders.Add(new MulticastSacnSenderIPV4());
			}
			if (configurationProvider.Ipv6Multicast)
			{
				multicastSenders.Add(new MulticastSacnSenderIPV6());
			}
			MulticastSenders = multicastSenders;

			var unicastSenders = new List<UnicastSacnSender>();
			foreach (var unicastTarget in configurationProvider.UnicastTargets)
			{
				unicastSenders.Add(new UnicastSacnSender(unicastTarget));
			}
			UnicastSenders = unicastSenders;
		}
		public async Task Render(ushort universeId, Memory<byte> data)
		{
			var packet = PacketFactory.CreateDataPacket(universeId, data.Span.ToArray());
			var tasks = UnicastSenders
				.Select(x => x.Send(packet))
				.Union(MulticastSenders.Select(x => x.Send(packet)));
			await Task.WhenAll(tasks);
		}
	}
}