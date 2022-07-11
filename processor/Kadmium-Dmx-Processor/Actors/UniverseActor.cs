using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Models;
using Kadmium_Dmx_Processor.Services.Renderer;

namespace Kadmium_Dmx_Processor.Actors
{
	public class UniverseActor : IDisposable
	{
		private IMemoryOwner<byte> DmxMemoryOwner { get; }
		private Memory<byte> DmxMemory { get; }
		public Universe Universe { get; }

		public Dictionary<ushort, FixtureActor> FixtureActors { get; } = new Dictionary<ushort, FixtureActor>();

		public UniverseActor(Universe universe)
		{
			DmxMemoryOwner = MemoryPool<byte>.Shared.Rent(Universe.MAX_SIZE);
			DmxMemory = DmxMemoryOwner.Memory.Slice(0, Universe.MAX_SIZE);
			Universe = universe;
		}

		public void Dispose()
		{
			DmxMemoryOwner.Dispose();
		}

		public async Task Render(IDmxRenderTarget renderTarget)
		{
			foreach (var fixture in FixtureActors.Values)
			{
				fixture.Render(DmxMemory);
			}

			await renderTarget.Send(Universe.UniverseId, DmxMemory);
		}
	}
}