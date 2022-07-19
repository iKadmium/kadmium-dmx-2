using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RtpMidiSource.Services.ValueCache
{
	public class AttributeValue
	{
		public AttributeValue(string group, string attribute)
		{
			Group = group;
			Attribute = attribute;
			Value = 0f;
			LastRenderedValue = null;
		}

		public string Group { get; set; }
		public string Attribute { get; set; }
		public float Value { get; set; }
		public float? LastRenderedValue { get; set; }
	}
}