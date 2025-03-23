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

using VitaNex.Text;
#endregion

namespace VitaNex.Notify
{
	public class NotifyGumpOption
	{
		public char? Prefix { get; set; }

		public TextDefinition Label { get; set; }
		public Action<GumpButton> Callback { get; set; }

		public Color LabelColor { get; set; }
		public Color FillColor { get; set; }
		public Color BorderColor { get; set; }

		public int Width => 10 + UOFont.GetUnicodeWidth(1, GetString());

		public NotifyGumpOption(TextDefinition label, Action<GumpButton> callback)
			: this(label, callback, Color.Empty)
		{ }

		public NotifyGumpOption(TextDefinition label, Action<GumpButton> callback, Color color)
			: this(label, callback, color, Color.Empty)
		{ }

		public NotifyGumpOption(TextDefinition label, Action<GumpButton> callback, Color color, Color fill)
			: this(label, callback, color, fill, Color.Empty)
		{ }

		public NotifyGumpOption(TextDefinition label, Action<GumpButton> callback, Color color, Color fill, Color border)
		{
			Prefix = UniGlyph.TriRightFill;

			Label = label;
			Callback = callback;

			LabelColor = color;
			FillColor = fill;
			BorderColor = border;
		}

		public override string ToString()
		{
			return GetString();
		}

		public string GetString()
		{
			return GetString(null);
		}

		public string GetString(Mobile viewer)
		{
			if (Prefix.HasValue)
			{
				return Prefix.Value + " " + Label.GetString(viewer);
			}

			return Label.GetString(viewer);
		}
	}
}