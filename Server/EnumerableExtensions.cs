// **********
// ServUO - EnumerableExtensions.cs
// **********

using System;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
	public static class EnumerableExtensions
	{
        public static void ForEach<T>(this IEnumerable source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            foreach (var t in source) action((T)t);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
            if (source == null) throw new ArgumentNullException(nameof(source));

			foreach (var t in source) action(t);
		}
	}
}