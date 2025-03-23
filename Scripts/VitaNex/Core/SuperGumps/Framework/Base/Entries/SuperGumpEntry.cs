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

using Server;
using Server.Gumps;
using Server.Network;
#endregion

namespace VitaNex.SuperGumps
{
	public interface IGumpEntryPoint
	{
		int X { get; set; }
		int Y { get; set; }
	}

	public interface IGumpEntrySize
	{
		int Width { get; set; }
		int Height { get; set; }
	}

	public interface IGumpEntryVector : IGumpEntryPoint, IGumpEntrySize
	{ }

	public abstract class SuperGumpEntry : GumpEntry, IDisposable
	{
		protected static readonly byte[] _EmptyLayout = Gump.StringToBuffer("null");

		protected void AppendEmptyLayout(IGumpWriter disp)
		{
			disp.AppendLayout(_EmptyLayout);
		}

		public virtual bool IgnoreModalOffset => false;

		public virtual Mobile User
		{
			get
			{
				if (Parent is SuperGump)
				{
					return ((SuperGump)Parent).User;
				}

				return null;
			}
		}

		public virtual NetState UserState
		{
			get
			{
				var user = User;

				if (user != null)
				{
					return user.NetState;
				}

				return null;
			}
		}

		public bool IsEnhancedClient => UserState != null && UserState.IsEnhanced();

		protected int FixHue(int hue)
		{
			return FixHue(hue, false);
		}

		protected int FixHue(int hue, bool item)
		{
			hue = Math.Max(-1, hue & 0x7FFF);

			if (hue <= 0)
			{
				return 0;
			}

			hue = Math.Min(3000, hue) - 1;

			if (item || IsEnhancedClient)
			{
				++hue;
			}

			return hue;
		}

		protected void Delta<T>(ref T var, T val)
		{
			if (ReferenceEquals(var, val) || Equals(var, val))
			{
				return;
			}

			var old = var;

			var = val;

			OnInvalidate(old, val);
			OnInvalidate();
		}

		protected virtual void OnInvalidate<T>(T old, T val)
		{ }

		protected virtual void OnInvalidate()
		{
			if (Parent != null)
			{
				Parent.Invalidate();
			}
		}

		public virtual void Dispose()
		{
			if (Parent != null)
			{
				Parent.Entries.Remove(this);
				Parent = null;
			}
		}
	}
}