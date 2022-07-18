using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RtpMidiSource.Services.Mqtt
{
	public interface IAttributeSetter
	{
		Task SetAttributeAsync(string group, string attribute, Memory<byte> value);

	}
}