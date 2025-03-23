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

using Server.Gumps;
using Server.Network;
#endregion

namespace VitaNex.SuperGumps
{
	public class GumpPixel : SuperGumpEntry, IGumpEntryVector
	{
		private const string _Format1 = "{{ htmlgump {0} {1} {2} {3} {4} 0 0 }}";

		private static readonly byte[] _Layout1 = Gump.StringToBuffer("htmlgump");

		private int _X, _Y;
		private Color _Color;

		public int X { get => _X; set => Delta(ref _X, value); }
		public int Y { get => _Y; set => Delta(ref _Y, value); }

		int IGumpEntrySize.Width { get => 1; set { } }
		int IGumpEntrySize.Height { get => 1; set { } }

		public Color Color { get => _Color; set => Delta(ref _Color, value); }

		public GumpPixel(int x, int y, Color color)
		{
			_X = x;
			_Y = y;

			_Color = color;
		}

		public override string Compile()
		{
			if (IsEnhancedClient)
			{
				return String.Empty;
			}

			var text = " ";

			if (!Color.IsEmpty && Color != Color.Transparent)
			{
				text = text.WrapUOHtmlBG(Color);
			}

			return String.Format(_Format1, X, Y, 1, 1, Parent.Intern(text));
		}

		public override void AppendTo(IGumpWriter disp)
		{
			if (IsEnhancedClient)
			{
				AppendEmptyLayout(disp);
				return;
			}

			var text = " ";

			if (!Color.IsEmpty && Color != Color.Transparent)
			{
				text = text.WrapUOHtmlBG(Color);
			}

			disp.AppendLayout(_Layout1);
			disp.AppendLayout(X);
			disp.AppendLayout(Y);
			disp.AppendLayout(1);
			disp.AppendLayout(1);
			disp.AppendLayout(Parent.Intern(text));
			disp.AppendLayout(false);
			disp.AppendLayout(false);
		}
	}
}