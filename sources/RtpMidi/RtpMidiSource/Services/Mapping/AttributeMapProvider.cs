using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using RtpMidiSource.Util;

namespace RtpMidiSource.Services.Mapping
{
	public class AttributeMapProvider : IAttributeMapProvider
	{
		private Cached<Dictionary<int, AttributeMapping>> AttributeMap { get; set; }

		public AttributeMapProvider(OptionsProvider optionsProvider)
		{
			AttributeMap = new Cached<Dictionary<int, AttributeMapping>>(async () =>
			{
				var options = await optionsProvider.GetOptionsAsync();
				var mapProperty = options.GetProperty("attributeMap");

				var serializerOptions = new JsonSerializerOptions();
				serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

				var map = mapProperty.Deserialize<Dictionary<int, AttributeMapping>>(serializerOptions);
				if (map == null)
				{
					throw new FileLoadException("Unable to parse options file");
				}
				return map;
			});
		}

		public async Task<Dictionary<int, AttributeMapping>> GetAttributeMapAsync()
		{
			var map = await AttributeMap.GetValue();
			if (map == null)
			{
				throw new FileLoadException("Unable to get Attribute map from options");
			}
			return map;
		}
	}
}