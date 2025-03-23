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
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class ConfirmDialogGump : DialogGump
	{
		public virtual bool Confirmed { get; set; }

		public ConfirmDialogGump(
			Mobile user,
			Gump parent = null,
			int? x = null,
			int? y = null,
			string title = null,
			string html = null,
			int icon = 7022,
			Action<GumpButton> onAccept = null,
			Action<GumpButton> onCancel = null)
			: base(user, parent, x, y, title, html, icon, onAccept, onCancel)
		{ }

		protected override void OnAccept(GumpButton button)
		{
			Confirmed = true;
			base.OnAccept(button);
		}

		protected override void OnCancel(GumpButton button)
		{
			Confirmed = false;
			base.OnCancel(button);
		}
	}
}