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
using Server;
using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPInviteGump : ConfirmDialogGump
	{
		public PvPInviteGump(Mobile user, PvPBattle battle, Gump parent = null)
			: base(user, parent, title: "Call To Arms")
		{
			Battle = battle;
		}

		public PvPBattle Battle { get; set; }

		protected override void Compile()
		{
			base.Compile();

			Html = Battle != null ? Battle.ToHtmlString(User) : "Battle does not exist.";
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Add(
				"textentry/body/question",
				() => AddLabelCropped(25, Height - 45, Width - 80, 20, HighlightHue, "Accept or Decline?"));
		}

		protected override void OnAccept(GumpButton button)
		{
			base.OnAccept(button);

			if (Battle != null && !Battle.Deleted)
			{
				Battle.AcceptInvite(User as PlayerMobile);
			}

			Close(true);
		}

		protected override void OnCancel(GumpButton button)
		{
			base.OnCancel(button);

			if (Battle != null && !Battle.Deleted)
			{
				Battle.DeclineInvite(User as PlayerMobile);
			}

			Close(true);
		}
	}
}