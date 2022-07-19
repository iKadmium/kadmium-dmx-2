using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RtpMidiSource.Services.Mapping;
using RtpMidiSource.Util;

namespace RtpMidiSource.Services.ValueCache
{
	public class ValueCache : IValueCache
	{
		private Dictionary<string, Dictionary<string, AttributeValue>> Values { get; set; } = new Dictionary<string, Dictionary<string, AttributeValue>>();
		private IAttributeMapProvider AttributeMapProvider { get; }
		private IGroupMapProvider GroupMapProvider { get; }

		public ValueCache(IAttributeMapProvider attributeMapProvider, IGroupMapProvider groupMapProvider)
		{
			AttributeMapProvider = attributeMapProvider;
			GroupMapProvider = groupMapProvider;
		}

		public async Task LoadAsync()
		{
			var attributeMap = await AttributeMapProvider.GetAttributeMapAsync();
			var groupMap = await GroupMapProvider.GetGroupMapAsync();

			var values = groupMap.Values.ToDictionary(
				groupName => groupName,
				groupName => attributeMap.Values.ToDictionary(
					attributeName => attributeName.Name,
					attributeName => new AttributeValue(groupName, attributeName.Name)
				)
			);

			Values.Clear();
			foreach (var value in values)
			{
				Values.Add(value.Key, value.Value);
			}
		}

		public IEnumerable<AttributeValue> GetDirtyAttributes()
		{
			var dirtyAttributes = from grp in Values.Values
								  from attribute in grp
								  where attribute.Value.LastRenderedValue != attribute.Value.Value
								  select attribute.Value;

			return dirtyAttributes.ToList();
		}

		public void OnRender()
		{
			foreach (var group in Values.Values)
			{
				foreach (var attribute in group.Values)
				{
					attribute.LastRenderedValue = attribute.Value;
				}
			}
		}

		public void SetValue(string group, string attribute, float value)
		{
			var thing = Values[group][attribute].Value = value;
		}
	}
}