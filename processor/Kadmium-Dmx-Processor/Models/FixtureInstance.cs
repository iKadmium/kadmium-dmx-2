using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.EffectRenderers;
using Kadmium_Dmx_Processor.Effects;

namespace Kadmium_Dmx_Processor.Models
{
	public class FixtureInstance
	{
		public FixtureInstance(ushort address, string manufacturer, string model, string personality)
		{
			Address = address;
			Manufacturer = manufacturer;
			Model = model;
			Personality = personality;
		}

		public ushort Address { get; }
		public string Manufacturer { get; }
		public string Model { get; }
		public string Personality { get; }

	}
}