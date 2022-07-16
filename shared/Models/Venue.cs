using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Shared.Models
{
	public class Venue : IHasId
	{
		public Venue(string id, string name, string city, Dictionary<ushort, Universe> universes)
			: this(name, city, universes)
		{
			Id = id;
		}

		[JsonConstructor]
		public Venue(string name, string city, Dictionary<ushort, Universe> universes)
		{
			Name = name;
			Universes = universes;
			City = city;
		}

		public string? Id { get; set; }
		public string Name { get; set; }
		public string City { get; set; }
		public Dictionary<ushort, Universe> Universes { get; set; }
	}
}