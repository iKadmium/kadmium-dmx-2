using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RtpMidiSource.Services.Mqtt
{
	public interface IAttributeSetter
	{
		void SetAttributeCached(string group, string attribute, float value);
		Task BeginAsync();
	}
}