using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RtpMidiSource.Services.Mapping
{
	public class AttributeMapProvider : IAttributeMapProvider
	{
		public Task<Dictionary<int, string>> GetAttributeMapAsync()
		{
			return Task.FromResult(new Dictionary<int, string>
			{
				{ 1, "Hue" },
				{ 2, "Saturation" },
				{ 3, "Brightness" }
			});
		}
	}
}