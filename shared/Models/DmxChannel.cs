using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Shared.Models
{
	public class DmxChannel
	{
		public DmxChannel(string name, byte min, byte max) : this(name)
		{
			Min = min;
			Max = max;
		}

		[JsonConstructor]
		public DmxChannel(string name)
		{
			Name = name;
		}

		public DmxChannel(DmxChannel channel) : this(channel.Name, channel.Min, channel.Max)
		{
		}

		public string Name { get; set; }
		public byte Min { get; set; } = 0;
		public byte Max { get; set; } = 255;
	}
}