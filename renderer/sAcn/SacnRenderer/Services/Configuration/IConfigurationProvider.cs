using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SacnRenderer.Services.Configuration
{
	public interface IConfigurationProvider
	{
		bool Ipv6Multicast { get; }
		bool Ipv4Multicast { get; }
		IEnumerable<IPAddress> UnicastTargets { get; }
		string MqttServer { get; }
	}
}