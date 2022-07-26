using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;

namespace Webapi.Models
{
	public record class Defaults : IHasId
	{
		public string? Id { get; set; }
		public string? VenueId { get; set; }
		public string? MidiMapId { get; set; }
	}
}