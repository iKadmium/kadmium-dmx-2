using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Effects;

namespace Kadmium_Dmx_Processor.Models
{
	public class Group
	{
		public Group(string name, IList<Fixture> fixtures)
		{
			Name = name;
			Fixtures = fixtures;
			Attributes = new Dictionary<string, EffectAttribute>();
		}

		public string Name { get; }
		public IList<Fixture> Fixtures { get; }
		public Dictionary<string, EffectAttribute> Attributes { get; }
	}
}