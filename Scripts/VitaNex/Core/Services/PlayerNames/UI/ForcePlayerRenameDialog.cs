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
using System.Linq;

using Server;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex
{
	public class ForcePlayerRenameDialog : InputDialogGump
	{
		public ForcePlayerRenameDialog(Mobile user, string input = null)
			: base(user, input: input, limit: 16)
		{
			CanClose = false;
			CanDispose = false;

			BlockMovement = true;
			BlockSpeech = true;

			Html = "It appears that another character is already using the name \"" + //
				   InputText.WrapUOHtmlColor(Color.LawnGreen, HtmlColor) + "\"!\n\n" + //
				   "Please enter a new name for your character...";
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Remove("button/body/cancel");
		}

		protected override void OnAccept(GumpButton button)
		{
			if (String.IsNullOrWhiteSpace(InputText) || !NameVerification.Validate(
					InputText,
					2,
					20,
					true,
					false,
					true,
					1,
					NameVerification.SpaceDashPeriodQuote))
			{
				Html = ("The name \"" + InputText + "\" is invalid.\n\n").WrapUOHtmlColor(Color.OrangeRed, HtmlColor) +
					   "It appears that another character is already using the name \"" + //
					   User.RawName.WrapUOHtmlColor(Color.LawnGreen, HtmlColor) + "\"!\n\n" + //
					   "Please enter a new name for your character...";

				InputText = NameList.RandomName(User.Female ? "female" : "male");

				Refresh(true);
				return;
			}

			if (InputText == User.RawName || PlayerNames.FindPlayers(
															InputText,
															p => p != User && p.GameTime > ((PlayerMobile)User).GameTime)
														.Any())
			{
				Html = "It appears that another character is already using the name \"" + //
					   InputText.WrapUOHtmlColor(Color.LawnGreen, HtmlColor) + "\"!\n\n" + //
					   "Please enter a new name for your character...";

				InputText = NameList.RandomName(User.Female ? "female" : "male");

				Refresh(true);
				return;
			}

			User.RawName = InputText;

			PlayerNames.Register((PlayerMobile)User);

			base.OnAccept(button);
		}
	}
}