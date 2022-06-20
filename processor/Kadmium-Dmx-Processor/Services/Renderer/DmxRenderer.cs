using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Models;
using Kadmium_Dmx_Processor.Services.Groups;
using Kadmium_Dmx_Processor.Services.Venues;

namespace Kadmium_Dmx_Processor.Services.Renderer
{
	public class DmxRenderer : IDmxRenderer, IDisposable
	{
		private IMemoryOwner<byte>? DmxMemory;
		private IVenueProvider VenueProvider { get; }
		private IGroupProvider GroupProvider { get; }
		private Venue? Venue { get; set; }
		private Dictionary<string, Group>? Groups { get; set; }

		public DmxRenderer(IVenueProvider venueProvider, IGroupProvider groupProvider)
		{
			VenueProvider = venueProvider;
			GroupProvider = groupProvider;
		}

		public async Task Load()
		{
			DmxMemory?.Dispose();
			Venue = await VenueProvider.GetVenueAsync();
			var memorySize = Venue.Universes.Count * Universe.MAX_SIZE;
			DmxMemory = MemoryPool<byte>.Shared.Rent(memorySize);
			Groups = await GroupProvider.GetGroups();
			foreach (var currentGroup in Groups.Values)
			{
				var fixturesInGroup = from universe in Venue.Universes.Values
									  from fixture in universe.Fixtures.Values
									  where fixture.Group == currentGroup.Name
									  select fixture;
				foreach (var fixture in fixturesInGroup)
				{
					currentGroup.Fixtures.Add(fixture);
				}
			}
		}

		public void Render()
		{
			if (Venue == null || DmxMemory == null)
			{
				throw new InvalidOperationException("No venue has been loaded");
			}

			foreach (var universe in Venue.Universes.Values)
			{
				foreach (var fixture in universe.Fixtures.Values)
				{
					foreach (var channel in fixture.Channels.Values)
					{
						var address = fixture.Address + channel.Address - 1;
						DmxMemory.Memory.Span[address] = channel.Value;
					}
				}
			}
		}

		public void Dispose()
		{
			DmxMemory?.Dispose();
		}
	}
}