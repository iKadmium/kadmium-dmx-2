using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Models
{
	public class Universe
	{
		public static int MAX_SIZE = 512;

		public Universe(string name, Dictionary<ushort, Fixture> fixtures)
		{
			Name = name;
			Fixtures = fixtures;
		}

		public string Name { get; }
		public Dictionary<ushort, Fixture> Fixtures { get; }
	}
}