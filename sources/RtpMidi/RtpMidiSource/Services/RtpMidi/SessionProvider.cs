using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using KadmiumRtpMidi;

namespace RtpMidiSource.Services.RtpMidi
{
	public class SessionProvider : ISessionProvider
	{
		public async Task<Session> GetSessionAsync()
		{
			var hostAddresses = await Dns.GetHostAddressesAsync(Dns.GetHostName());
			var session = new Session(hostAddresses.First(), 5023, "kadmium-dmx Receiver");
			return session;
		}
	}
}