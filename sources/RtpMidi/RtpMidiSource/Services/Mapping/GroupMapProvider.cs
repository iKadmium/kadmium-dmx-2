using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RtpMidiSource.Services.Mapping
{
	public class GroupMapProvider : IGroupMapProvider
	{
		public Task<Dictionary<int, string>> GetGroupMapAsync()
		{
			return Task.FromResult(new Dictionary<int, string>
			{
				{ 1, "vocalist" },
				{ 2, "guitarist" }
			});
		}
	}
}