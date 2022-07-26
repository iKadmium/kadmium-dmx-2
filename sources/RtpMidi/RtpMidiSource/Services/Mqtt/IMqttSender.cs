using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MQTTnet.Client;

namespace RtpMidiSource.Services.Mqtt
{
	public interface IMqttSender
	{
		Task Begin();
		Task PublishAsync(string topic, Memory<byte> payload);
		Task SubscribeAsync(string topic, Func<MqttApplicationMessageReceivedEventArgs, Task> handler);
	}
}