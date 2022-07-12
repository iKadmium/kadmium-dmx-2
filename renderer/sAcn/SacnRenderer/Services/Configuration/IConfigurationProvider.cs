using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SacnRenderer.Services.Configuration
{
	public interface IConfigurationProvider
	{
		public bool Ipv6Multicast { get; }
		public bool Ipv4Multicast { get; }
		public IEnumerable<IPAddress> UnicastTargets { get; }
	}
}