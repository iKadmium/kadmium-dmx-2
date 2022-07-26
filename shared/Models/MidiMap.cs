using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Shared.Models
{
	public record MidiMap : IHasId
	{
		public MidiMap(string name, Dictionary<byte, string> channelGroups, Dictionary<byte, AttributeMapping> ccAttributes)
		{
			Name = name;
			ChannelGroups = channelGroups;
			CcAttributes = ccAttributes;
		}

		public string? Id { get; set; }
		public string Name { get; set; }
		public Dictionary<byte, string> ChannelGroups { get; set; }
		public Dictionary<byte, AttributeMapping> CcAttributes { get; set; }
	}
}