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

		public async Task SetAttributeAsync<T>(string group, string attribute, T value)
		{
			var topic = $"groups/{group}/{attribute}";
			var payload = value?.ToString();

			if (payload == null)
			{
				throw new ArgumentNullException("Value was null");
			}

			await ConnectionProvider.PublishAsync(topic, payload);
		}
	}
}