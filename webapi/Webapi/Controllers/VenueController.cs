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
		private IFixtureDefinitionProvider FixtureDefinitionProvider { get; }

		public VenueController(
			ILogger<VenueController> logger,
			ICrudProvider<VenueKey, Venue> crudProvider,
			IFixtureDefinitionProvider fixtureDefinitionProvider
		) : base(logger, crudProvider)
		{
			FixtureDefinitionProvider = fixtureDefinitionProvider;
		}

		[HttpGet("{id}/payload")]
		public async Task<VenuePayload> GetPayload([FromRoute] string id)
		{
			var venue = await Read(id);
			var instances = venue.Universes.Values.SelectMany(x => x.Fixtures.Values);
			var ids = await GetIdsForInstances(instances);
			var definitions = await FixtureDefinitionProvider.Read(ids);
			var groups = instances.SelectMany(x => x.Groups).Distinct();

			var payload = new VenuePayload(groups, definitions, venue);
			return payload;
		}

		private async Task<IEnumerable<string>> GetIdsForInstances(IEnumerable<FixtureInstance> instances)
		{
			var allKeys = await FixtureDefinitionProvider.ReadKeys();
			var ids = new List<string>();
			foreach (var instance in instances)
			{
				var key = allKeys.Single(key => key.Manufacturer == instance.Manufacturer && key.Model == instance.Model);
				if (key != null && key.Id != null && !ids.Contains(key.Id))
				{
					ids.Add(key.Id);
				}
			}
			return ids;
		}
	}
}