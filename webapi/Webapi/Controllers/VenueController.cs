using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Webapi.Models;
using Webapi.Services;

namespace Webapi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class VenueController : CrudControllerBase<VenueKey, Venue>
	{
		private IVenueProvider VenueProvider { get; }

		public VenueController(
			ILogger<VenueController> logger,
			IVenueProvider crudProvider,
			IFixtureDefinitionProvider fixtureDefinitionProvider
		) : base(logger, crudProvider)
		{
			VenueProvider = crudProvider;
		}

		[HttpPost("{id}/activate")]
		public async Task Activate([FromRoute] string id)
		{
			await VenueProvider.Activate(id);
		}
	}
}