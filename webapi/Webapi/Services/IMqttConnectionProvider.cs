using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MQTTnet.Client;

namespace Webapi.Services
{
	public interface IMqttConnectionProvider
	{
		Task Begin();
		Task PublishAsync(string topic, Memory<byte> payload, bool retain = true);
	}
}