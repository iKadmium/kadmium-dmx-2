using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MQTTnet.Client;

namespace RtpMidiSource.Services.Mqtt
{
	public interface IMqttSender
	{
		Task PublishAsync(string topic, string payload);
	}
}