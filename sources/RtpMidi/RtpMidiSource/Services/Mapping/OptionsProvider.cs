using System.Text.Json;
using RtpMidiSource.Util;

namespace RtpMidiSource.Services.Mapping
{
	public class OptionsProvider : IOptionsProvider
	{
		private const string filename = @"data/options.json";
		private Cached<JsonDocument> Options { get; }

		public OptionsProvider()
		{
			Options = new Cached<JsonDocument>(async () =>
			{
				if (!File.Exists(filename))
				{
					throw new FileNotFoundException("Could not locate options file");
				}

				var text = File.OpenRead(filename);
				if (text == null)
				{
					throw new FileLoadException("Unable to open options file");
				}
				var doc = await JsonDocument.ParseAsync(text);
				if (doc == null)
				{
					throw new JsonException("Unable to parse options file");
				}
				return doc;
			});
		}

		public async Task<JsonElement> GetOptionsAsync()
		{
			var options = await Options.GetValue();
			if (options == null)
			{
				throw new FileLoadException("Unable to get options");
			}
			return options.RootElement;
		}
	}
}