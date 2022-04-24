using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RtpMidiSource.Services.Mapping
{
	public interface IAttributeMapProvider
	{
		Task<Dictionary<int, AttributeMapping>> GetAttributeMapAsync();
	}
}