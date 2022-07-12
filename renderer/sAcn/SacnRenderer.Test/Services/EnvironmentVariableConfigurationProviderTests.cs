using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SacnRenderer.Services.Configuration;

namespace SacnRenderer.Test.Services
{
	public class EnvironmentVariableConfigurationProviderTests
	{
		[Theory]
		[InlineData("TRUE", true)]
		[InlineData("true", true)]
		[InlineData(null, false)]
		[InlineData("FALSE", false)]
		[InlineData("false", false)]
		public void When_Ipv6MulticastIsCalled_Then_TheResultsAreAsExpected(string? value, bool expected)
		{
			Environment.SetEnvironmentVariable("IPV6_MULTICAST", value);
			var provider = new EnvironmentVariableConfigurationProvider();
			Assert.Equal(expected, provider.Ipv6Multicast);
		}

		[Theory]
		[InlineData("TRUE", true)]
		[InlineData("true", true)]
		[InlineData(null, true)]
		[InlineData("FALSE", false)]
		[InlineData("false", false)]
		public void When_Ipv4MulticastIsCalled_Then_TheResultsAreAsExpected(string? value, bool expected)
		{
			Environment.SetEnvironmentVariable("IPV4_MULTICAST", value);
			var provider = new EnvironmentVariableConfigurationProvider();
			Assert.Equal(expected, provider.Ipv4Multicast);
		}

		[Theory]
		[InlineData("192.168.0.1", "192.168.0.1")]
		[InlineData("192.168.0.1, 127.0.0.1", "192.168.0.1", "127.0.0.1")]
		[InlineData("192.168.0.1,127.0.0.1", "192.168.0.1", "127.0.0.1")]
		[InlineData(null)]
		public void When_UnicastTargetsIsCalled_Then_TheResultsAreAsExpected(string? value, params string[] expectedAddresses)
		{
			Environment.SetEnvironmentVariable("UNICAST_TARGETS", value);
			var provider = new EnvironmentVariableConfigurationProvider();
			var expected = expectedAddresses.Select(x => IPAddress.Parse(x));
			Assert.Equal(expected, provider.UnicastTargets);
		}
	}
}