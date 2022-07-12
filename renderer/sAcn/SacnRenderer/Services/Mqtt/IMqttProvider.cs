using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SacnRenderer.Services.Mqtt
{
	public interface IMqttProvider
	{
		event EventHandler<MqttEvent> MqttEventReceived;
		Task Connect();
	}
}