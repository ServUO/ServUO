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
using System.Drawing;

using Server;
using Server.Gumps;

using VitaNex.Text;
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class HtmlPanelGump<T> : PanelGump<T>
	{
		public virtual string Html { get; set; }
		public virtual Color HtmlColor { get; set; }
		public virtual bool HtmlBackground { get; set; }

		public HtmlPanelGump(
			Mobile user,
			Gump parent = null,
			int? x = null,
			int? y = null,
			int width = 420,
			int height = 420,
			string html = null,
			string emptyText = null,
			string title = null,
			IEnumerable<ListGumpEntry> opts = null,
			T selected = default(T))
			: base(user, parent, x, y, width, height, emptyText, title, opts, selected)
		{
			HtmlColor = DefaultHtmlColor;
			HtmlBackground = false;

			Html = html ?? String.Empty;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			if (Minimized)
			{
				return;
			}

			layout.Add(
				"html/body/base",
				() =>
				{
					var html = Html.ParseBBCode(HtmlColor);

					AddHtml(15, 65, Width - 30, Height - 30, html, HtmlBackground, true);
				});
		}
	}
}