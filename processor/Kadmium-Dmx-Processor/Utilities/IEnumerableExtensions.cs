using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadmium_Dmx_Processor.Utilities
{
	public static class IEnumerableExtensions
	{
		public static bool ContainsAll<T>(this IEnumerable<T> collection, params T[] items)
		{
			return items.All(x => collection.Contains(x));
		}
	}
}