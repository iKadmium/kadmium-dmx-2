using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Shared.Models
{
	public class FixtureDefinition : IHasId
	{
		public FixtureDefinition(string id, string manufacturer, string model, Dictionary<string, Dictionary<ushort, DmxChannel>> personalities, Dictionary<string, Axis> movementAxis)
		: this(manufacturer, model, personalities, movementAxis)
		{
			Id = id;
		}

		[JsonConstructor]
		public FixtureDefinition(string manufacturer, string model, Dictionary<string, Dictionary<ushort, DmxChannel>> personalities, Dictionary<string, Axis> movementAxis)
		{
			Manufacturer = manufacturer;
			Model = model;
			Personalities = personalities;
			MovementAxis = movementAxis;
		}

		public string? Id { get; set; }
		public string Manufacturer { get; set; }
		public string Model { get; set; }
		public Dictionary<string, Dictionary<ushort, DmxChannel>> Personalities { get; set; }
		public Dictionary<string, Axis> MovementAxis { get; set; }
	}
}