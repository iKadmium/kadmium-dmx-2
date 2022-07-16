using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Shared.Models
{
	public interface IHasId
	{
		string? Id { get; set; }
	}
}