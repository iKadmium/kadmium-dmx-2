using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RtpMidiSource.Services.ValueCache
{
	public interface IValueCache
	{
		void SetValue(string group, string attribute, float value);
		IEnumerable<AttributeValue> GetDirtyAttributes();
		void OnRender();
		Task LoadAsync();
	}
}