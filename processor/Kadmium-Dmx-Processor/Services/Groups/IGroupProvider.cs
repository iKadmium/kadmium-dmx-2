using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Shared.Models;

namespace Kadmium_Dmx_Processor.Services.Groups
{
	public interface IGroupProvider
	{
		Dictionary<string, Group> Groups { get; }
	}
}