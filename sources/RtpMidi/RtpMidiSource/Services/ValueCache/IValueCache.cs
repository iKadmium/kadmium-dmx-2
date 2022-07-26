using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Shared.Models;

namespace RtpMidiSource.Services.ValueCache
{
	public interface IValueCache
	{
		void SetValue(byte channel, byte ccNumber, byte value);
		IEnumerable<AttributeValue> GetDirtyAttributes();
		void OnRender();
		void Load(MidiMap map);
	}
}