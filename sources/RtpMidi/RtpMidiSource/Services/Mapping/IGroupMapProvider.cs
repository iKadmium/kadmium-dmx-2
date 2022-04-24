using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RtpMidiSource.Services.Mapping
{
	public interface IGroupMapProvider
	{
		Task<Dictionary<int, string>> GetGroupMapAsync();
	}
}