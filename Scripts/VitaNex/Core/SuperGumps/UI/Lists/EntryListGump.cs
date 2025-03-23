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
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class EntryListGump : ListGump<ListGumpEntry>
	{
		public EntryListGump(
			Mobile user,
			Gump parent,
			int? x = null,
			int? y = null,
			IEnumerable<ListGumpEntry> list = null,
			string emptyText = null,
			string title = null,
			IEnumerable<ListGumpEntry> opts = null)
			: base(user, parent, x, y, list, emptyText, title, opts)
		{ }

		protected override void SelectEntry(GumpButton button, ListGumpEntry entry)
		{
			base.SelectEntry(button, entry);

			if (entry.Handler != null)
			{
				entry.Handler(button);
			}
		}

		protected override int GetLabelHue(int index, int pageIndex, ListGumpEntry entry)
		{
			return entry != null ? entry.Hue : base.GetLabelHue(index, pageIndex, ListGumpEntry.Empty);
		}

		protected override string GetLabelText(int index, int pageIndex, ListGumpEntry entry)
		{
			return entry != null && !String.IsNullOrWhiteSpace(entry.Label)
				? entry.Label
				: base.GetLabelText(index, pageIndex, ListGumpEntry.Empty);
		}
	}
}