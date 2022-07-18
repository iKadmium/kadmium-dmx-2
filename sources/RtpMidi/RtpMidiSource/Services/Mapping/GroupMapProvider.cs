using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using RtpMidiSource.Util;

namespace RtpMidiSource.Services.Mapping
{
	public class GroupMapProvider : IGroupMapProvider
	{
		private Cached<Dictionary<int, string>> GroupMap { get; set; }

		public GroupMapProvider(IOptionsProvider optionsProvider)
		{
			GroupMap = new Cached<Dictionary<int, string>>(async () =>
			{
				var options = await optionsProvider.GetOptionsAsync();
				var mapProperty = options.GetProperty("groupMap");

				var map = mapProperty.Deserialize<Dictionary<int, string>>();
				if (map == null)
				{
					throw new FileLoadException("Unable to parse options file");
				}
				return map;
			});
		}

		public async Task<Dictionary<int, string>> GetGroupMapAsync()
		{
			var map = await GroupMap.GetValue();
			if (map == null)
			{
				throw new FileLoadException("Unable to get group map from options");
			}
			return map;
		}
	}
}