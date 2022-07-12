using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_sACN;
using Kadmium_sACN.SacnSender;

namespace SacnRenderer.Services.Sacn
{
	public class SacnRenderer : ISacnRenderer
	{
		private SacnPacketFactory PacketFactory { get; }
		private MulticastSacnSender Sender { get; }
		private UnicastSacnSender UnicastSender { get; }

		public SacnRenderer()
		{
			PacketFactory = new Kadmium_sACN.SacnPacketFactory(new byte[] { 1, 2, 3, 4 }, "Kadmium-DMX");
			Sender = new MulticastSacnSenderIPV4();
			UnicastSender = new UnicastSacnSender(System.Net.IPAddress.Parse("192.168.0.69"));
		}
		public async Task Render(ushort universeId, Memory<byte> data)
		{
			var packet = PacketFactory.CreateDataPacket(universeId, data.Span.ToArray());
			await UnicastSender.Send(packet);
			await Sender.Send(packet);
		}
	}
}