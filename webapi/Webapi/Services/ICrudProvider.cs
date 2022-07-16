using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;

namespace Webapi.Services
{
	public interface ICrudProvider<TKey, TObject>
		where TKey : IHasId
		where TObject : IHasId
	{
		Task<string> Create(TObject obj);
		Task<IEnumerable<TKey>> ReadKeys();
		Task<TObject> Read(string id);
		Task<TObject> Update(string id, TObject obj);
		Task Delete(string id);
		Task<IEnumerable<TKey>> Search(string query);
	}
}