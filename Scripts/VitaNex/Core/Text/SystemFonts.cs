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
using System.Drawing;
#endregion

namespace VitaNex.Text
{
	public enum SystemFont
	{
		Default,
		Arial,
		Tahoma,
		Terminal,
		Verdana
	}

	public static class Fonts
	{
		public static readonly Font Default = new Font("Arial", 12);
		public static readonly Font Arial = new Font("Arial", 12);
		public static readonly Font Tahoma = new Font("Tahoma", 12);
		public static readonly Font Terminal = new Font("Terminal", 12);
		public static readonly Font Verdana = new Font("Verdana", 12);

		public static Font Dupe(this Font font, float emSize = 12, FontStyle style = FontStyle.Regular)
		{
			return new Font(font.FontFamily.Name, emSize, style);
		}

		public static Font ToFont(this SystemFont font, float emSize = 12, FontStyle style = FontStyle.Regular)
		{
			switch (font)
			{
				case SystemFont.Arial:
					return Dupe(Arial, emSize, style);
				case SystemFont.Tahoma:
					return Dupe(Tahoma, emSize, style);
				case SystemFont.Terminal:
					return Dupe(Terminal, emSize, style);
				case SystemFont.Verdana:
					return Dupe(Verdana, emSize, style);
				default:
					return Dupe(Default, emSize, style);
			}
		}
	}
}