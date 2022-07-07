using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Models;
using Kadmium_Dmx_Processor.Utilities;

namespace Kadmium_Dmx_Processor.EffectRenderers.Movement
{
	public class Movement16BitRenderer : IEffectRenderer
	{
		public IEnumerable<string> RenderTargets { get; }
		public string AxisName { get; }

		public Movement16BitRenderer(string axisName)
		{
			RenderTargets = new[] { $"{axisName}Fine", $"{axisName}Coarse" };
			AxisName = axisName;
		}

		public void Render(Dictionary<string, EffectAttribute> pipeline, Dictionary<ushort, DmxChannel> channels)
		{
			var rawValue = pipeline[AxisName].Value;
			var shortVal = (ushort)Scale.Rescale(rawValue, 0, 1, ushort.MinValue, ushort.MaxValue);
			var bytes = BitConverter.GetBytes(shortVal);
			var coarse = channels.Values.Single(x => x.Name == $"{AxisName}Coarse");
			var fine = channels.Values.Single(x => x.Name == $"{AxisName}Fine");
			coarse.Value = bytes[1];
			fine.Value = bytes[0];
		}
	}
}