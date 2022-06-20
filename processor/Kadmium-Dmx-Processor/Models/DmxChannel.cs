using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Models
{
	public class DmxChannel
	{
		public DmxChannel(string name, ushort address)
		{
			Name = name;
			Address = address;
			Value = 0;
		}

		public DmxChannel(DmxChannel channel) : this(channel.Name, channel.Address)
		{
		}

		public string Name { get; }
		public ushort Address { get; }
		public byte Value { get; set; }
	}
}