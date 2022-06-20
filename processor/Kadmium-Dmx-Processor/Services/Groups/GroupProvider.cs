using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Models;

namespace Kadmium_Dmx_Processor.Services.Groups
{
	public class GroupProvider : IGroupProvider
	{
		public Task<Dictionary<string, Group>> GetGroups()
		{
			var groups = new Dictionary<string, Group>
			{
				{
					"vocalist", new Group(
						"vocalist",
						new List<Fixture>()
					)
				}
			};
			return Task.FromResult(groups);
		}
	}
}