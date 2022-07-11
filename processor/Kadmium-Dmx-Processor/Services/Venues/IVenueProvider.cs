using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Processor.Models;

namespace Kadmium_Dmx_Processor.Services.Venues
{
	public interface IVenueProvider
	{
		void LoadVenue(JsonDocument document);
		Venue Venue { get; }
		Dictionary<ushort, UniverseActor> UniverseActors { get; }
	}
}