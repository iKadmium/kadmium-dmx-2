using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;
using Webapi.Models;

namespace Webapi.Services
{
	public interface IVenueProvider : ICrudProvider<VenueKey, Venue>
	{
		Task Activate(string id);
	}
}