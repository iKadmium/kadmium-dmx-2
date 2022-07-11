using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Processor.Models;

namespace Kadmium_Dmx_Processor.Services.Groups
{
	public interface IGroupProvider
	{
		Task LoadGroups();
		Dictionary<string, Group> Groups { get; }

		void Clear();
	}
}