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

using Server.Gumps;
#endregion

namespace VitaNex.SuperGumps.UI
{
	public struct ListGumpEntry : IEquatable<ListGumpEntry>, IEquatable<string>
	{
		public static ListGumpEntry Empty = new ListGumpEntry(String.Empty, () => { });

		private static Action<GumpButton> Resolve(Action handler)
		{
			return b =>
			{
				if (handler != null)
				{
					handler();
				}
			};
		}

		public static bool IsNullOrEmpty(ListGumpEntry entry)
		{
			return entry == null || entry == Empty;
		}

		public string Label { get; set; }
		public int Hue { get; set; }
		public Action<GumpButton> Handler { get; set; }

		public ListGumpEntry(string label, Action handler)
			: this(label, Resolve(handler), SuperGump.DefaultTextHue)
		{ }

		public ListGumpEntry(string label, Action handler, int hue)
			: this(label, Resolve(handler), hue)
		{ }

		public ListGumpEntry(string label, Action<GumpButton> handler)
			: this(label, handler, SuperGump.DefaultTextHue)
		{ }

		public ListGumpEntry(string label, Action<GumpButton> handler, int hue)
			: this()
		{
			Label = label;
			Handler = handler;
			Hue = hue;
		}

		public override string ToString()
		{
			return Label;
		}

		public override int GetHashCode()
		{
			return Label != null ? Label.GetHashCode() : 0;
		}

		public override bool Equals(object obj)
		{
			return (obj is string && Equals((string)obj)) || (obj is ListGumpEntry && Equals((ListGumpEntry)obj));
		}

		public bool Equals(ListGumpEntry other)
		{
			return Label == other.Label;
		}

		public bool Equals(string other)
		{
			return Label == other;
		}

		public static bool operator ==(ListGumpEntry l, ListGumpEntry r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(ListGumpEntry l, ListGumpEntry r)
		{
			return !l.Equals(r);
		}

		public static bool operator ==(ListGumpEntry l, string r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(ListGumpEntry l, string r)
		{
			return !l.Equals(r);
		}

		public static bool operator ==(string l, ListGumpEntry r)
		{
			return r.Equals(l);
		}

		public static bool operator !=(string l, ListGumpEntry r)
		{
			return !r.Equals(l);
		}
	}
}