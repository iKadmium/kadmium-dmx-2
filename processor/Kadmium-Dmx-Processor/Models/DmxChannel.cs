using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Models
{
	public class DmxChannel
	{
		public DmxChannel(string name, ushort address, byte min, byte max) : this(name, address)
		{
			Min = min;
			Max = max;
		}

		[JsonConstructor]
		public DmxChannel(string name, ushort address)
		{
			Name = name;
			Address = address;
		}

		public DmxChannel(DmxChannel channel) : this(channel.Name, channel.Address)
		{
		}

		public string Name { get; }
		public ushort Address { get; }
		public byte Min { get; } = 0;
		public byte Max { get; } = 255;
	}
}