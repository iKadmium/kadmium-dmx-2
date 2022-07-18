using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;
using Kadmium_Dmx_Processor.Services.Groups;
using Kadmium_Dmx_Processor.Services.Venues;
using Kadmium_Dmx_Processor.Services.Configuration;
using Kadmium_Dmx_Processor.Services.TimeProvider;

namespace Kadmium_Dmx_Processor.Services.Renderer
{
	public class DmxRenderer : IDmxRenderer
	{
		private IVenueProvider VenueProvider { get; }
		private IGroupProvider GroupProvider { get; }
		private IDmxRenderTarget RenderTarget { get; }
		private IConfigurationProvider ConfigurationProvider { get; }
		private Timer? RenderTimer { get; set; }
		private ITimeProvider TimeProvider { get; }

		public DmxRenderer(IVenueProvider venueProvider, IGroupProvider groupProvider, IDmxRenderTarget renderTarget, IConfigurationProvider configurationProvider, ITimeProvider timeProvider)
		{
			VenueProvider = venueProvider;
			GroupProvider = groupProvider;
			RenderTarget = renderTarget;
			ConfigurationProvider = configurationProvider;
			TimeProvider = timeProvider;
		}

		public async Task Render()
		{
			TimeProvider.OnRender();
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

		public void Start()
		{
			if (RenderTimer != null)
			{
				RenderTimer.Dispose();
			}
			RenderTimer = new Timer(async (state) =>
			{
				await Render();
			}, null, 0, (1000 / ConfigurationProvider.RefreshRate));
		}
	}
}