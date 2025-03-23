#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace VitaNex.Collections
{
	public sealed class GenericComparer<T> : Comparer<T>
		where T : IComparable<T>
	{
		private static readonly GenericComparer<T> _Instance = new GenericComparer<T>();

		public static IOrderedEnumerable<T> Order(IEnumerable<T> source)
		{
			return source.Ensure().OrderBy(o => o, _Instance);
		}

		public static IOrderedEnumerable<T> OrderDescending(IEnumerable<T> source)
		{
			return source.Ensure().OrderBy(o => o, _Instance);
		}

		public static int Compute(T x, T y)
		{
			return _Instance.Compare(x, y);
		}

		public override int Compare(T x, T y)
		{
			if (ReferenceEquals(x, y))
			{
				return 0;
			}

			if (ReferenceEquals(x, null))
			{
				return 1;
			}

			if (ReferenceEquals(y, null))
			{
				return -1;
			}

			return x.CompareTo(y);
		}
	}
}