using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webapi.Models;
using Webapi.Services;

namespace Webapi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DefaultsController : ControllerBase
	{
		private IDefaultsProvider DefaultsService { get; }
		private ILogger<DefaultsController> Logger { get; }

		public DefaultsController(IDefaultsProvider defaultsService, ILogger<DefaultsController> logger)
		{
			DefaultsService = defaultsService;
			Logger = logger;
		}

		[HttpGet]
		public async Task<Defaults> Index()
		{
			return await DefaultsService.GetDefaults();
		}

		[HttpPut]
		public async Task<Defaults> Update([FromBody] Defaults defaults)
		{
			return await DefaultsService.UpdateDefaults(defaults);
		}

		[HttpPut("midimap/{id}")]
		public async Task<Defaults> SetDefaultMidiMap([FromRoute] string id)
		{
			var defaults = await DefaultsService.GetDefaults();
			defaults.MidiMapId = id;
			return await DefaultsService.UpdateDefaults(defaults);
		}

		[HttpPut("venue/{id}")]
		public async Task<Defaults> SetDefaultVenue([FromRoute] string id)
		{
			var defaults = await DefaultsService.GetDefaults();
			defaults.VenueId = id;
			return await DefaultsService.UpdateDefaults(defaults);
		}
	}
}