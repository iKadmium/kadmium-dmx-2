using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MQTTnet;

namespace RtpMidiSource.Services.Mqtt
{
	public class MqttAttributeSetter : IAttributeSetter
	{
		private IMqttSender ConnectionProvider { get; }

		public MqttAttributeSetter(IMqttSender connectionProvider)
		{
			ConnectionProvider = connectionProvider;
		}

		public async Task SetAttributeAsync(string group, string attribute, Memory<byte> value)
		{
			var topic = $"/group/{group}/{attribute}";
			var payload = value;

			await ConnectionProvider.PublishAsync(topic, payload);
		}
	}
}