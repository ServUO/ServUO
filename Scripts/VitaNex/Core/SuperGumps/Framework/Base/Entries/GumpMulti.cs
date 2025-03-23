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
using System.Drawing;

using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

using Ultima;

using MultiComponentList = Server.MultiComponentList;
#endregion

namespace VitaNex.SuperGumps
{
	public class GumpMulti : SuperGumpEntry, IGumpEntryPoint
	{
		private const string _Format1 = "{{ tilepic {0} {1} {2} }}";
		private const string _Format2 = "{{ tilepichue {0} {1} {2} {3} }}";

		private static readonly byte[] _Layout1A = Gump.StringToBuffer("tilepic");
		private static readonly byte[] _Layout1B = Gump.StringToBuffer(" }{ tilepic");
		private static readonly byte[] _Layout2A = Gump.StringToBuffer("tilepichue");
		private static readonly byte[] _Layout2B = Gump.StringToBuffer(" }{ tilepichue");

		private int _X, _Y;
		private int _Hue;
		private MultiComponentList _Components;

		public int X { get => _X; set => Delta(ref _X, value); }
		public int Y { get => _Y; set => Delta(ref _Y, value); }

		public int Hue { get => _Hue; set => Delta(ref _Hue, value); }

		public MultiComponentList Components { get => _Components; set => Delta(ref _Components, value); }

		public Point Offset
		{
			get
			{
				var o = Components.GetImageOffset();

				o.X = _X + (o.X < 0 ? Math.Abs(o.X) : o.X);
				o.Y = _Y + (o.Y < 0 ? Math.Abs(o.Y) : o.Y);

				return o;
			}
		}

		private Size? _Size;

		public Size Size => _Size ?? (_Size = Components.GetImageSize()).Value;

		public int Width => Size.Width;
		public int Height => Size.Height;

		public GumpMulti(int x, int y, BaseMulti multi)
			: this(x, y, multi.Hue, multi.GetComponents())
		{ }

		public GumpMulti(int x, int y, int multiID)
			: this(x, y, 0, multiID)
		{ }

		public GumpMulti(int x, int y, int hue, int multiID)
			: this(x, y, hue, MultiExtUtility.GetComponents(multiID))
		{ }

		public GumpMulti(int x, int y, MultiComponentList components)
			: this(x, y, 0, components)
		{ }

		public GumpMulti(int x, int y, int hue, MultiComponentList components)
		{
			_X = x;
			_Y = y;
			_Hue = hue;
			_Components = components ?? MultiComponentList.Empty;
		}

		protected override void OnInvalidate<T>(T old, T val)
		{
			base.OnInvalidate(old, val);

			if (val is MultiComponentList)
			{
				_Size = null;
			}
		}

		public override string Compile()
		{
			var compiled = String.Empty;

			if (_Components.List.IsNullOrEmpty())
			{
				if (_Hue > 0)
				{
					compiled += String.Format(_Format2, _X, _Y, 1, FixHue(_Hue, true));
				}
				else
				{
					compiled += String.Format(_Format1, _X, _Y, 1);
				}

				return compiled;
			}

			var o = Offset;

			Components.EnumerateByRender(
				(p, t) =>
				{
					if (_Hue > 0)
					{
						compiled += String.Format(_Format2, o.X + p.X, o.Y + p.Y, t.m_ItemID, FixHue(_Hue, true));
					}
					else
					{
						compiled += String.Format(_Format1, o.X + p.X, o.Y + p.Y, t.m_ItemID);
					}
				});

			return compiled;
		}

		public override void AppendTo(IGumpWriter disp)
		{
			if (_Components.List.IsNullOrEmpty())
			{
				disp.AppendLayout(_Hue > 0 ? _Layout2A : _Layout1A);
				disp.AppendLayout(_X);
				disp.AppendLayout(_Y);
				disp.AppendLayout(1);

				if (_Hue > 0)
				{
					disp.AppendLayout(FixHue(_Hue, true));
				}

				return;
			}

			var o = Offset;

			var first = true;

			Components.EnumerateByRender(
				(p, t) =>
				{
					if (first)
					{
						disp.AppendLayout(_Hue > 0 ? _Layout2A : _Layout1A);
					}
					else
					{
						disp.AppendLayout(_Hue > 0 ? _Layout2B : _Layout1B);
					}

					disp.AppendLayout(o.X + p.X);
					disp.AppendLayout(o.Y + p.Y);
					disp.AppendLayout(t.m_ItemID);

					if (_Hue > 0)
					{
						disp.AppendLayout(FixHue(_Hue, true));
					}

					first = false;
				});
		}
	}
}