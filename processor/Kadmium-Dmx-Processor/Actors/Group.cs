using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Effects;

namespace Kadmium_Dmx_Processor.Actors
{
	public class Group
	{
		public Group(string name, List<FixtureActor> fixtures)
		{
			Name = name;
			Fixtures = fixtures;
		}

		public string Name { get; }
		public List<FixtureActor> Fixtures { get; }
		public Dictionary<string, EffectAttribute> Pipeline { get; } = new Dictionary<string, EffectAttribute>();
		public List<IEffect> Effects { get; } = new List<IEffect>();

		public void Render()
		{
			foreach (var effect in Effects)
			{
				effect.Apply(Pipeline);
			}
			foreach (var fixture in Fixtures)
			{
				//fixture.Render();
			}
		}
	}
}