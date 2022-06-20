using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Models;

namespace Kadmium_Dmx_Processor.Services.Fixtures
{
	public interface IFixtureDefinitionProvider
	{
		Task<FixtureDefinition> GetFixtureDefinition(string manufacturer, string model);
	}
}