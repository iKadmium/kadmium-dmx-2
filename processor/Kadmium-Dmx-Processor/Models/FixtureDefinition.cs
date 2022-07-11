using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Models
{
	public class FixtureDefinition
	{
		public FixtureDefinition(string manufacturer, string model, Dictionary<string, Dictionary<ushort, DmxChannel>> personalities, Dictionary<string, Axis> movementAxis)
		{
			Manufacturer = manufacturer;
			Model = model;
			Personalities = personalities;
			MovementAxis = movementAxis;
		}

		public string Manufacturer { get; }
		public string Model { get; }
		public Dictionary<string, Dictionary<ushort, DmxChannel>> Personalities { get; }
		public Dictionary<string, Axis> MovementAxis { get; }
	}
}