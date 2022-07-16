using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Services.Mqtt
{
	public interface IMqttProvider
	{
		event EventHandler<MqttEvent> MqttEventReceived;
		Task Send(string topic, Memory<byte> packet, bool retain = false);
		Task Subscribe(params string[] topics);
		Task Subscribe(IEnumerable<string> topics);
		Task UnsubscribeAll();
		Task Connect();
	}
}