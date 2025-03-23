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

using Server;
using Server.Gumps;
#endregion

namespace VitaNex.SuperGumps
{
	public abstract class SuperGumpDictionary<T1, T2> : SuperGumpPages
	{
		private static Dictionary<T1, T2> Acquire(IDictionary<T1, T2> dictionary = null)
		{
			return dictionary != null ? new Dictionary<T1, T2>(dictionary) : new Dictionary<T1, T2>(0x100);
		}

		public Dictionary<T1, T2> List { get; set; }

		public override int EntryCount => List.Count;

		public SuperGumpDictionary(
			Mobile user,
			Gump parent = null,
			int? x = null,
			int? y = null,
			IDictionary<T1, T2> dictionary = null)
			: base(user, parent, x, y)
		{
			List = Acquire(dictionary);
		}

		protected override void Compile()
		{
			if (List == null)
			{
				List = Acquire();
			}

			CompileList(List);

			base.Compile();
		}

		public Dictionary<int, KeyValuePair<T1, T2>> GetListRange()
		{
			return GetListRange(Page * EntriesPerPage, EntriesPerPage);
		}

		public Dictionary<int, KeyValuePair<T1, T2>> GetListRange(int index, int length)
		{
			index = Math.Max(0, Math.Min(EntryCount, index));
			length = Math.Max(0, Math.Min(EntryCount - index, length));

			var d = new Dictionary<int, KeyValuePair<T1, T2>>(length);

			while (--length >= 0 && List.InBounds(index))
			{
				d[index] = List.ElementAtOrDefault(index);

				++index;
			}

			return d;
		}

		protected virtual void CompileList(Dictionary<T1, T2> list)
		{ }

		protected override void OnDispose()
		{
			base.OnDispose();

			if (List != null)
			{
				List.Clear();
			}
		}

		protected override void OnDisposed()
		{
			base.OnDisposed();

			List = null;
		}
	}
}