using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Shared.Models
{
	public class Universe
	{
		public static int MAX_SIZE = 512;
		public string Name { get; set; }
		public Dictionary<ushort, FixtureInstance> Fixtures { get; set; }

		public Universe(string name, Dictionary<ushort, FixtureInstance> fixtures)
		{
			Name = name;
			Fixtures = fixtures;
		}
	}
}