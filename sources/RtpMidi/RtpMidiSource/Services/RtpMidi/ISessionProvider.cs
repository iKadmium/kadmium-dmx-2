using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KadmiumRtpMidi;

namespace RtpMidiSource.Services.RtpMidi
{
	public interface ISessionProvider
	{
		Task<Session> GetSessionAsync();
	}
}