using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.EffectRenderers;
using Kadmium_Dmx_Processor.Effects;

namespace Kadmium_Dmx_Processor.Models
{
	public class FixtureInstance
	{
		[JsonConstructor]
		public FixtureInstance(ushort address, string manufacturer, string model, string personality, IEnumerable<string> groups, Dictionary<string, JsonElement> options)
		{
			Address = address;
			Manufacturer = manufacturer;
			Model = model;
			Personality = personality;
			Groups = groups;
			Options = options;
		}

		public FixtureInstance(ushort address, string manufacturer, string model, string personality)
		{
			Address = address;
			Manufacturer = manufacturer;
			Model = model;
			Personality = personality;
			Groups = new List<string>();
			Options = new Dictionary<string, JsonElement>();
		}

		public ushort Address { get; }
		public string Manufacturer { get; }
		public string Model { get; }
		public string Personality { get; }
		public IEnumerable<string> Groups { get; }
		public Dictionary<string, JsonElement> Options { get; }
	}
}