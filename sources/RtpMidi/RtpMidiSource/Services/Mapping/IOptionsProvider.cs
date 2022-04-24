using System.Text.Json;

namespace RtpMidiSource.Services.Mapping
{
	public interface IOptionsProvider
	{
		public Task<JsonElement> GetOptionsAsync();
	}
}
