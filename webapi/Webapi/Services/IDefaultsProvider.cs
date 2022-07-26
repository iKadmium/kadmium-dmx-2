using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webapi.Models;

namespace Webapi.Services
{
	public interface IDefaultsProvider
	{
		Task<Defaults> GetDefaults();
		Task<Defaults> UpdateDefaults(Defaults defaults);
	}
}