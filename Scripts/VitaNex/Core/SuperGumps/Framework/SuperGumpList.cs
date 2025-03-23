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

using Server;
using Server.Gumps;

using VitaNex.Collections;
#endregion

namespace VitaNex.SuperGumps
{
	public abstract class SuperGumpList<T> : SuperGumpPages
	{
		public static List<T> Acquire(IEnumerable<T> e = null)
		{
			var o = ListPool<T>.AcquireObject();

			if (e != null)
			{
				o.AddRange(e);
			}
			else if (o.Capacity < 0x40)
			{
				o.Capacity = 0x40;
			}

			return o;
		}

		public static void Free(List<T> o)
		{
			ListPool<T>.FreeObject(o);
		}

		public virtual List<T> List { get; set; }

		public override int EntryCount => List.Count;

		public virtual bool Sorted { get; set; }

		public SuperGumpList(Mobile user, Gump parent = null, int? x = null, int? y = null, IEnumerable<T> list = null)
			: base(user, parent, x, y)
		{
			List = Acquire(list);
		}

		protected override void Compile()
		{
			if (List == null)
			{
				List = Acquire();
			}

			CompileList(List);

			if (Sorted)
			{
				Sort();
			}

			base.Compile();
		}

		public virtual IEnumerable<T> EnumerateListRange()
		{
			return EnumerateListRange(Page * EntriesPerPage, EntriesPerPage);
		}

		public virtual IEnumerable<T> EnumerateListRange(int index, int length)
		{
			index = Math.Max(0, Math.Min(EntryCount, index));
			length = Math.Max(0, Math.Min(EntryCount - index, length));

			while (--length >= 0 && List.InBounds(index))
			{
				yield return List[index++];
			}
		}

		public virtual Dictionary<int, T> GetListRange()
		{
			return GetListRange(Page * EntriesPerPage, EntriesPerPage);
		}

		public virtual Dictionary<int, T> GetListRange(int index, int length)
		{
			index = Math.Max(0, Math.Min(EntryCount, index));
			length = Math.Max(0, Math.Min(EntryCount - index, length));

			var d = new Dictionary<int, T>(length);

			while (--length >= 0 && List.InBounds(index))
			{
				d[index] = List[index++];
			}

			return d;
		}

		protected virtual void CompileList(List<T> list)
		{ }

		public virtual int SortCompare(T a, T b)
		{
			if (a is IComparable<T> ac)
			{
				return ac.CompareTo(b);
			}

			if (b is IComparable<T> bc)
			{
				return bc.CompareTo(a);
			}

			return a.CompareNull(b);
		}

		public virtual void Sort()
		{
			List.Sort(SortCompare);
		}

		public virtual void Sort(Comparison<T> comparison)
		{
			if (comparison != null)
			{
				List.Sort(comparison);
				return;
			}

			Sort();
		}

		public virtual void Sort(IComparer<T> comparer)
		{
			if (comparer != null)
			{
				List.Sort(comparer);
				return;
			}

			Sort();
		}

		protected override void OnClosed(bool all)
		{
			base.OnClosed(all);

			if (!IsOpen)
			{
				List.Free(false);
			}
		}

		protected override void OnDispose()
		{
			base.OnDispose();

			if (List != null)
			{
				List.Free(true);
			}
		}

		protected override void OnDisposed()
		{
			base.OnDisposed();

			if (List == null)
			{
				return;
			}

			var o = List;

			List = null;

			Free(o);
		}
	}
}
