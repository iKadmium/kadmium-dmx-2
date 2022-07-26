using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Shared.Models
{
	public record AttributeMapping
	{
		private const byte MidiMin = (byte)0;
		private const byte MidiMax = (byte)127;

		public string Name { get; set; }
		public float Min { get; set; } = 0;
		public float Max { get; set; } = 1;

		public AttributeMapping(string name)
		{
			Name = name;
		}

		public float GetAdjustedValue(byte rawValue)
		{
			var range = Max - Min;
			var percentage = (float)rawValue / MidiMax;
			var value = percentage * range + Min;
			return value;
		}
	}
}