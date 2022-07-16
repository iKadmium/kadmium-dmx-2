using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;

namespace Webapi.Models
{
	public class FixtureDefinitionKey : IHasId
	{
		public FixtureDefinitionKey(string id, string manufacturer, string model)
		{
			Id = id;
			Manufacturer = manufacturer;
			Model = model;
		}

		public string? Id { get; set; }
		public string Manufacturer { get; set; }
		public string Model { get; set; }
	}
}