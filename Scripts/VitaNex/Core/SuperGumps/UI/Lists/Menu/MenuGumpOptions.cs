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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Server.Gumps;
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class MenuGumpOptions : IEnumerable<ListGumpEntry>, IEquatable<MenuGumpOptions>
	{
		private readonly List<ListGumpEntry> _Options;

		public int Count => _Options.Count;

		public ListGumpEntry this[int index] { get => GetEntryAt(index); set => Replace(value); }
		public ListGumpEntry this[string label] { get => GetEntry(label); set => Replace(label, value); }

		public MenuGumpOptions()
		{
			_Options = new List<ListGumpEntry>(0x10);
		}

		public MenuGumpOptions(IEnumerable<ListGumpEntry> options)
			: this()
		{
			AppendRange(options);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<ListGumpEntry> GetEnumerator()
		{
			return _Options.GetEnumerator();
		}

		public void Clear()
		{
			_Options.Clear();
		}

		public void AppendRange(IEnumerable<ListGumpEntry> options)
		{
			if (options != null)
			{
				options.ForEach(AppendEntry);
			}
		}

		public void AppendEntry(string label, Action handler)
		{
			AppendEntry(new ListGumpEntry(label, handler));
		}

		public void AppendEntry(string label, Action handler, int hue)
		{
			AppendEntry(new ListGumpEntry(label, handler, hue));
		}

		public void AppendEntry(string label, Action<GumpButton> handler)
		{
			AppendEntry(new ListGumpEntry(label, handler));
		}

		public void AppendEntry(string label, Action<GumpButton> handler, int hue)
		{
			AppendEntry(new ListGumpEntry(label, handler, hue));
		}

		public void AppendEntry(ListGumpEntry entry)
		{
			Insert(Count, entry);
		}

		public void PrependRange(IEnumerable<ListGumpEntry> options)
		{
			if (options != null)
			{
				options.ForEach(PrependEntry);
			}
		}

		public void PrependEntry(string label, Action handler)
		{
			PrependEntry(new ListGumpEntry(label, handler));
		}

		public void PrependEntry(string label, Action handler, int hue)
		{
			PrependEntry(new ListGumpEntry(label, handler, hue));
		}

		public void PrependEntry(string label, Action<GumpButton> handler)
		{
			PrependEntry(new ListGumpEntry(label, handler));
		}

		public void PrependEntry(string label, Action<GumpButton> handler, int hue)
		{
			PrependEntry(new ListGumpEntry(label, handler, hue));
		}

		public void PrependEntry(ListGumpEntry entry)
		{
			Insert(0, entry);
		}

		public bool RemoveEntry(ListGumpEntry entry)
		{
			return _Options.RemoveAll(o => o == entry) > 0;
		}

		public bool RemoveEntry(string label)
		{
			return _Options.RemoveAll(o => o == label) > 0;
		}

		public ListGumpEntry GetEntryAt(int index)
		{
			return _Options.ElementAtOrDefault(index);
		}

		public ListGumpEntry GetEntry(string label)
		{
			return _Options.Find(o => o == label);
		}

		public int IndexOfEntry(ListGumpEntry entry)
		{
			return _Options.IndexOf(o => o == entry);
		}

		public int IndexOfLabel(string label)
		{
			return _Options.IndexOf(o => o == label);
		}

		public void Insert(int index, ListGumpEntry entry)
		{
			if (ListGumpEntry.IsNullOrEmpty(entry))
			{
				return;
			}

			index = Math.Max(0, Math.Min(Count, index));

			var i = IndexOfEntry(entry);

			if (i != -1)
			{
				_Options.RemoveAt(i);

				if (index > i)
				{
					--index;
				}
			}

			_Options.Insert(index, entry);
		}

		public void Replace(ListGumpEntry entry)
		{
			Replace(entry.Label, entry);
		}

		public void Replace(string label, ListGumpEntry entry)
		{
			var i = IndexOfLabel(label);

			if (i == -1 || (RemoveEntry(label) && i > Count))
			{
				i = Count;
			}

			Insert(i, entry);
		}

		public void Replace(string label, Action handler)
		{
			Replace(label, new ListGumpEntry(label, handler));
		}

		public void Replace(string label, Action handler, int hue)
		{
			Replace(label, new ListGumpEntry(label, handler, hue));
		}

		public void Replace(string search, string label, Action handler)
		{
			Replace(search, new ListGumpEntry(label, handler));
		}

		public void Replace(string search, string label, Action handler, int hue)
		{
			Replace(search, new ListGumpEntry(label, handler, hue));
		}

		public void Replace(string label, Action<GumpButton> handler)
		{
			Replace(label, new ListGumpEntry(label, handler));
		}

		public void Replace(string label, Action<GumpButton> handler, int hue)
		{
			Replace(label, new ListGumpEntry(label, handler, hue));
		}

		public void Replace(string search, string label, Action<GumpButton> handler)
		{
			Replace(search, new ListGumpEntry(label, handler));
		}

		public void Replace(string search, string label, Action<GumpButton> handler, int hue)
		{
			Replace(search, new ListGumpEntry(label, handler, hue));
		}

		public bool Contains(string label)
		{
			return IndexOfLabel(label) != -1;
		}

		public bool Contains(ListGumpEntry entry)
		{
			return IndexOfEntry(entry) != -1;
		}

		public override int GetHashCode()
		{
			return _Options.Aggregate(Count, (hash, e) => unchecked((hash * 397) ^ e.GetHashCode()));
		}

		public override bool Equals(object obj)
		{
			return obj is MenuGumpOptions && Equals((MenuGumpOptions)obj);
		}

		public virtual bool Equals(MenuGumpOptions other)
		{
			return !ReferenceEquals(other, null) && GetHashCode() == other.GetHashCode();
		}

		public static bool operator ==(MenuGumpOptions l, MenuGumpOptions r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(MenuGumpOptions l, MenuGumpOptions r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}
	}
}