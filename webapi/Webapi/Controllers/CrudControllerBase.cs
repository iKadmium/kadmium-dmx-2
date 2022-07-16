using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Webapi.Services;

namespace Webapi.Controllers
{
	public abstract class CrudControllerBase<TKey, TObject>
	where TKey : IHasId
	where TObject : IHasId
	{
		private ILogger<VenueController> Logger { get; }
		private ICrudProvider<TKey, TObject> CrudProvider { get; }

		public CrudControllerBase(ILogger<VenueController> logger, ICrudProvider<TKey, TObject> crudProvider)
		{
			Logger = logger;
			CrudProvider = crudProvider;
		}

		[HttpGet]
		public async Task<IEnumerable<TKey>> ReadKeys()
		{
			return await CrudProvider.ReadKeys();
		}

		[HttpGet("{id}")]
		public async Task<TObject> Read([FromRoute] string id)
		{
			return await CrudProvider.Read(id);
		}

		[HttpPost]
		public async Task<string> CreateObject([FromBody] TObject obj)
		{
			return await CrudProvider.Create(obj);
		}

		[HttpPut("{id}")]
		public async Task<TObject> Update([FromRoute] string id, [FromBody] TObject obj)
		{
			return await CrudProvider.Update(id, obj);
		}

		[HttpDelete("{id}")]
		public async Task Delete([FromRoute] string id)
		{
			await CrudProvider.Delete(id);
		}
	}
}