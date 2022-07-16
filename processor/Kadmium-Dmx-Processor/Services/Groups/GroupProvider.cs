using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;
using Kadmium_Dmx_Processor.Actors;

namespace Kadmium_Dmx_Processor.Services.Groups
{
	public class GroupProvider : IGroupProvider
	{
		public Dictionary<string, Group> Groups { get; } = new Dictionary<string, Group>();
	}
}