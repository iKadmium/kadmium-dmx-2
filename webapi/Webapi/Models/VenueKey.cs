using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;

namespace Webapi.Models
{
	public class VenueKey : IHasId
	{
		public VenueKey(string id, string name, string city)
		{
			Id = id;
			Name = name;
			City = city;
		}

		public string? Id { get; set; }
		public string Name { get; set; }
		public string City { get; set; }
	}
}