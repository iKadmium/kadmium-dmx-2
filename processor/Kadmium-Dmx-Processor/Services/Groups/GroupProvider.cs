using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Models;
using Kadmium_Dmx_Processor.Actors;

namespace Kadmium_Dmx_Processor.Services.Groups
{
	public class GroupProvider : IGroupProvider
	{
		public Dictionary<string, Group> Groups { get; private set; } = new Dictionary<string, Group>();

		public void Clear()
		{
			foreach (var group in Groups.Values)
			{
				group.Fixtures.Clear();
			}
		}

		public Task LoadGroups()
		{
			Groups = new Dictionary<string, Group>
			{
				{
					"vocalist", new Group(
						"vocalist",
						new List<FixtureActor>()
					)
				}
			};
			return Task.CompletedTask;
		}
	}
}