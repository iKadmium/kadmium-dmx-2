using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SacnRenderer.Services.Configuration
{
	public class EnvironmentVariableConfigurationProvider : IConfigurationProvider
	{
		public bool Ipv6Multicast { get; } = bool.Parse(Environment.GetEnvironmentVariable("IPV6_MULTICAST") ?? false.ToString());

		public bool Ipv4Multicast { get; } = bool.Parse(Environment.GetEnvironmentVariable("IPV4_MULTICAST") ?? true.ToString());

		public IEnumerable<IPAddress> UnicastTargets { get; } = (Environment.GetEnvironmentVariable("UNICAST_TARGETS") ?? "")
			.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(x => IPAddress.Parse(x));
	}
}