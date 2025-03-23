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

using System.Collections.Concurrent;

namespace VitaNex.Collections
{
	public sealed class ConcurrentBagPool<T> : ObjectPool<ConcurrentBag<T>>
	{
		public ConcurrentBagPool()
		{ }

		public ConcurrentBagPool(int capacity)
			: base(capacity)
		{ }

		protected override bool Sanitize(ConcurrentBag<T> o)
		{
			while (!o.IsEmpty)
			{
				o.TryTake(out _);
			}

			return o.Count == 0;
		}
	}
}
