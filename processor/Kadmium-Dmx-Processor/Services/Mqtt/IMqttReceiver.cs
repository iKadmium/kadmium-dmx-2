using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Services.Mqtt
{
	public interface IMqttReceiver
	{
		event EventHandler<MqttEvent> MqttEventReceived;
		Task Connect();

	}
}