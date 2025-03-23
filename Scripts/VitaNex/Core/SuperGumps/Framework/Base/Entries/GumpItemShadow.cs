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
	public class GumpItemShadow : SuperGumpEntry, IGumpEntryPoint
	{
		private const string _Format1 = "{{ tilepic {0} {1} {2} }}";
		private const string _Format2 = "{{ tilepichue {0} {1} {2} {3} }}";

		private static readonly byte[] _Layout1A = Gump.StringToBuffer("tilepic");
		private static readonly byte[] _Layout1B = Gump.StringToBuffer(" }{ tilepic");
		private static readonly byte[] _Layout2A = Gump.StringToBuffer("tilepichue");
		private static readonly byte[] _Layout2B = Gump.StringToBuffer(" }{ tilepichue");

		private int _X, _Y;
		private int _ItemID, _ItemHue;
		private Angle _ShadowAngle;
		private int _ShadowOffset, _ShadowHue;

		public int X { get => _X; set => Delta(ref _X, value); }
		public int Y { get => _Y; set => Delta(ref _Y, value); }

		public int ItemID { get => _ItemID; set => Delta(ref _ItemID, value); }
		public int ItemHue { get => _ItemHue; set => Delta(ref _ItemHue, value); }

		public Angle ShadowAngle { get => _ShadowAngle; set => Delta(ref _ShadowAngle, value); }
		public int ShadowOffset { get => _ShadowOffset; set => Delta(ref _ShadowOffset, value); }
		public int ShadowHue { get => _ShadowHue; set => Delta(ref _ShadowHue, value); }

		public GumpItemShadow(int x, int y, int itemID)
			: this(x, y, itemID, 0)
		{ }

		public GumpItemShadow(int x, int y, int itemID, int itemHue)
			: this(x, y, itemID, itemHue, 45)
		{ }

		public GumpItemShadow(int x, int y, int itemID, int itemHue, Angle shadowAngle)
			: this(x, y, itemID, itemHue, shadowAngle, 5)
		{ }

		public GumpItemShadow(int x, int y, int itemID, int itemHue, Angle shadowAngle, int shadowOffset)
			: this(x, y, itemID, itemHue, shadowAngle, shadowOffset, 2999)
		{ }

		public GumpItemShadow(int x, int y, int itemID, int itemHue, Angle shadowAngle, int shadowOffset, int shadowHue)
		{
			_X = x;
			_Y = y;
			_ItemID = itemID;
			_ItemHue = itemHue;
			_ShadowAngle = shadowAngle;
			_ShadowOffset = shadowOffset;
			_ShadowHue = shadowHue;
		}

		public override string Compile()
		{
			var compiled = String.Empty;

			var s = _ShadowAngle.GetPoint2D(_X, _Y, _ShadowOffset);

			if (_ShadowHue <= 0)
			{
				compiled += String.Format(_Format1, s.X, s.Y, _ItemID);
			}
			else
			{
				compiled += String.Format(_Format2, s.X, s.Y, _ItemID, FixHue(_ShadowHue, true));
			}

			if (_ItemHue <= 0)
			{
				compiled += String.Format(_Format1, _X, _Y, _ItemID);
			}
			else
			{
				compiled += String.Format(_Format2, _X, _Y, _ItemID, FixHue(_ItemHue, true));
			}

			return compiled;
		}

		public override void AppendTo(IGumpWriter disp)
		{
			var s = _ShadowAngle.GetPoint2D(_X, _Y, _ShadowOffset);

			disp.AppendLayout(_ShadowHue > 0 ? _Layout2A : _Layout1A);
			disp.AppendLayout(s.X);
			disp.AppendLayout(s.Y);
			disp.AppendLayout(_ItemID);

			if (_ShadowHue > 0)
			{
				disp.AppendLayout(FixHue(_ShadowHue, true));
			}

			disp.AppendLayout(_ItemHue > 0 ? _Layout2B : _Layout1B);
			disp.AppendLayout(_X);
			disp.AppendLayout(_Y);
			disp.AppendLayout(_ItemID);

			if (_ItemHue > 0)
			{
				disp.AppendLayout(FixHue(_ItemHue, true));
			}
		}
	}
}