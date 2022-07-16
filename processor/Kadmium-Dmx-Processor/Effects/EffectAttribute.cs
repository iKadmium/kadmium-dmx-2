using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Effects
{
	public class EffectAttribute
	{
		public float Value { get; set; }
		public bool InternalOnly { get; set; } = false;
	}
}