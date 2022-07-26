using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;

namespace Webapi.Models
{
	public record MidiMapKey : IHasId
	{
		public MidiMapKey(string id, string name)
		{
			Id = id;
			Name = name;
		}

		public string? Id { get; set; }
		public string Name { get; set; }
	}
}