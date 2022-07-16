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
	public class VenueController : CrudControllerBase<VenueKey, Venue>
	{
		public VenueController(ILogger<VenueController> logger, ICrudProvider<VenueKey, Venue> crudProvider) : base(logger, crudProvider)
		{
		}
	}
}