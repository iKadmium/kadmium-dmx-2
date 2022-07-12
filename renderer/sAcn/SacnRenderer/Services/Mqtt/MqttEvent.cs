using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SacnRenderer.Services.Mqtt
{
	public class MqttEvent
	{
		public string Topic { get; set; }
		public byte[] Payload { get; set; }

		public MqttEvent(string topic, byte[] payload)
		{
			Topic = topic;
			Payload = payload;
		}
	}
}