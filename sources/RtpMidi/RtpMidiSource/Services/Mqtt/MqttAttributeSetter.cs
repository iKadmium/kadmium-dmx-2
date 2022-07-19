using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MQTTnet;
using RtpMidiSource.Services.ValueCache;

namespace RtpMidiSource.Services.Mqtt
{
	public class MqttAttributeSetter : IAttributeSetter
	{
		public const int UPDATE_RATE_HZ = 45;
		public const int RENDER_TIME_MS = 1000 / UPDATE_RATE_HZ;

		private IMqttSender ConnectionProvider { get; }
		private IValueCache ValueCache { get; }
		private Timer? RenderThread { get; set; }

		public MqttAttributeSetter(IMqttSender connectionProvider, IValueCache valueCache)
		{
			ConnectionProvider = connectionProvider;
			ValueCache = valueCache;
		}

		public async Task BeginAsync()
		{
			if (RenderThread != null)
			{
				RenderThread.Dispose();
			}
			await ConnectionProvider.Begin();

			RenderThread = new Timer(async (state) =>
			{
				var dirtyAttributes = ValueCache.GetDirtyAttributes();
				var promises = dirtyAttributes.Select(x => SetAttributeAsync(x.Group, x.Attribute, BitConverter.GetBytes(x.Value)));
				ValueCache.OnRender();
				await Task.WhenAll(promises);
			}, null, RENDER_TIME_MS, RENDER_TIME_MS);
		}

		public void SetAttributeCached(string group, string attribute, float value)
		{
			ValueCache.SetValue(group, attribute, value);
		}

		private async Task SetAttributeAsync(string group, string attribute, Memory<byte> value)
		{
			var topic = $"/group/{group}/{attribute}";
			var payload = value;

			await ConnectionProvider.PublishAsync(topic, payload);
		}
	}
}