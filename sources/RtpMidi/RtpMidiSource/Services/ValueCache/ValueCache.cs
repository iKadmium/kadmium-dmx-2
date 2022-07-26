using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;
using Microsoft.Extensions.Logging;
using RtpMidiSource.Services.Mqtt;
using RtpMidiSource.Util;

namespace RtpMidiSource.Services.ValueCache
{
	public class ValueCache : IValueCache
	{
		private Dictionary<string, Dictionary<string, AttributeValue>> Values { get; set; } = new Dictionary<string, Dictionary<string, AttributeValue>>();
		private MidiMap MidiMap { get; set; } = new MidiMap("No map", new Dictionary<byte, string>(), new Dictionary<byte, AttributeMapping>());

		public ValueCache(IMqttSender mqttConnectionProvider, ILogger<ValueCache> logger)
		{
			mqttConnectionProvider.SubscribeAsync("/midimap/load", async (message) =>
			{
				var map = JsonSerializer.Deserialize<MidiMap>(message.ApplicationMessage.Payload, new JsonSerializerOptions(JsonSerializerDefaults.Web));
				if (map != null)
				{
					logger.LogInformation("Loading midimap");
					MidiMap = map;
					await message.AcknowledgeAsync(CancellationToken.None);
				}
			});
		}

		public void Load(MidiMap map)
		{
			MidiMap = map;

			var values = map.ChannelGroups.Values.ToDictionary(
				groupName => groupName,
				groupName => map.ChannelGroups.Values.ToDictionary(
					attributeName => attributeName,
					attributeName => new AttributeValue(groupName, attributeName)
				)
			);

			Values.Clear();
			foreach (var value in values)
			{
				Values.Add(value.Key, value.Value);
			}
		}

		public IEnumerable<AttributeValue> GetDirtyAttributes()
		{
			var dirtyAttributes = from grp in Values.Values
								  from attribute in grp
								  where attribute.Value.LastRenderedValue != attribute.Value.Value
								  select attribute.Value;

			return dirtyAttributes.ToList();
		}

		public void OnRender()
		{
			foreach (var group in Values.Values)
			{
				foreach (var attribute in group.Values)
				{
					attribute.LastRenderedValue = attribute.Value;
				}
			}
		}

		public void SetValue(string group, string attribute, float value)
		{
			Values[group][attribute].Value = value;
		}

		public void SetValue(byte channel, byte ccNumber, byte value)
		{
			if (MidiMap.CcAttributes.ContainsKey(ccNumber) && MidiMap.ChannelGroups.ContainsKey(channel))
			{
				var attribute = MidiMap.CcAttributes[ccNumber];
				var group = MidiMap.ChannelGroups[channel];
				var adjustedValue = attribute.GetAdjustedValue(value);

				Values[group][attribute.Name].Value = adjustedValue;
			}
		}
	}
}