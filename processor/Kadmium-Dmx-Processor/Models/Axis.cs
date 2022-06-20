using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Models
{
	public class Axis
	{
		public Axis(string name, float minAngle, float maxAngle)
		{
			Name = name;
			MinAngle = minAngle;
			MaxAngle = maxAngle;
		}

		public Axis(Axis other) : this(other.Name, other.MinAngle, other.MaxAngle)
		{
		}

		public string Name { get; }
		public float MinAngle { get; }
		public float MaxAngle { get; }
	}
}