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

		public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> collection, int count)
		{
			var length = collection.Count();
			if (count > length)
			{
				throw new ArgumentOutOfRangeException("Count was greater than the size of the collection");
			}
			var indices = Enumerable.Range(0, count).Select(x => Random.Shared.Next(length)).OrderBy(x => x);
			var items = indices.Select(x => collection.Skip(x).Take(1).Single());
			return items;
		}
	}
}