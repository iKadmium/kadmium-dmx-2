using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Services.Renderer;

namespace Kadmium_Dmx_Processor.Models
{
	public class Universe
	{
		public static int MAX_SIZE = 512;
		public string Name { get; }
		public Dictionary<ushort, FixtureInstance> Fixtures { get; }

		public Universe(string name, Dictionary<ushort, FixtureInstance> fixtures)
		{
			Name = name;
			Fixtures = fixtures;
		}
	}
}