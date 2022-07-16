using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;
using Kadmium_Dmx_Processor.Services.Groups;
using Kadmium_Dmx_Processor.Services.Venues;

namespace Kadmium_Dmx_Processor.Services.Renderer
{
	public class DmxRenderer : IDmxRenderer
	{
		private IVenueProvider VenueProvider { get; }
		private IGroupProvider GroupProvider { get; }
		private IDmxRenderTarget RenderTarget { get; }

		public DmxRenderer(IVenueProvider venueProvider, IGroupProvider groupProvider, IDmxRenderTarget renderTarget)
		{
			VenueProvider = venueProvider;
			GroupProvider = groupProvider;
			RenderTarget = renderTarget;
		}

		public async Task Render()
		{
			foreach (var group in GroupProvider.Groups.Values)
			{
				group.Clear();
				group.Draw();
			}

			var promises = VenueProvider.UniverseActors.Select(x =>
			{
				x.Value.Clear();
				x.Value.Draw();
				return x.Value.Render(RenderTarget, x.Key);
			});

			await Task.WhenAll(promises);
		}
	}
}