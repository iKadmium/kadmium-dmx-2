using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Models
{
	public class Venue
	{
		public Venue(string name, Dictionary<ushort, Universe> universes)
		{
			Name = name;
			Universes = universes;
		}

		public string Name { get; }
		public Dictionary<ushort, Universe> Universes { get; }
	}
}