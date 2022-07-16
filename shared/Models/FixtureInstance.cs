using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Shared.Models
{
	public class FixtureInstance
	{
		[JsonConstructor]
		public FixtureInstance(string manufacturer, string model, string personality, IEnumerable<string> groups, Dictionary<string, object> options)
			: this(manufacturer, model, personality)
		{
			Groups = groups;
			Options = options;
		}

		public FixtureInstance(string manufacturer, string model, string personality)
		{
			Manufacturer = manufacturer;
			Model = model;
			Personality = personality;
			Groups = new List<string>();
			Options = new Dictionary<string, object>();
		}

		public string Manufacturer { get; set; }
		public string Model { get; set; }
		public string Personality { get; set; }
		public IEnumerable<string> Groups { get; set; }
		public Dictionary<string, object> Options { get; set; }
	}
}