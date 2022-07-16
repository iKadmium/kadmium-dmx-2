using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Services.Mqtt
{
	public interface IMqttEventHandler
	{
		Task Handle(MqttEvent mqttEvent);
	}
}