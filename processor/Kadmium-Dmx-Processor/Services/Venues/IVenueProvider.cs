using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Models;

namespace Kadmium_Dmx_Processor.Services.Venues
{
	public interface IVenueProvider
	{
		Task<Venue> GetVenueAsync();
	}
}