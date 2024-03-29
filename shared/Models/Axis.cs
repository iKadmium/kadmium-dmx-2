using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Shared.Models
{
	public class Axis
	{
		[JsonConstructor]
		public Axis(float minAngle, float maxAngle)
		{
			MinAngle = minAngle;
			MaxAngle = maxAngle;
		}

		public Axis(Axis other) : this(other.MinAngle, other.MaxAngle)
		{
		}

		public float MinAngle { get; set; }
		public float MaxAngle { get; set; }
	}
}