using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SacnRenderer.Services.Sacn
{
	public interface ISacnRenderer
	{
		public Task Render(ushort universeId, Memory<byte> data);
	}
}