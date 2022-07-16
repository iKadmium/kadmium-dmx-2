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
	[Route("[controller]")]
	public class FixtureDefinitionController : CrudControllerBase<FixtureDefinitionKey, FixtureDefinition>
	{
		public FixtureDefinitionController(ILogger<VenueController> logger, ICrudProvider<FixtureDefinitionKey, FixtureDefinition> crudProvider) : base(logger, crudProvider)
		{
		}
	}
}