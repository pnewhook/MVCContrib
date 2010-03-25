using System;
using System.Collections.Generic;

namespace MvcContrib.CommandProcessor.Helpers
{
	internal static class EnumerableExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach(var item in items)
			{
				action(item);
			}
		}
	}
}