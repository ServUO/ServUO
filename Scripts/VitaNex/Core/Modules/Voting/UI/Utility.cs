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
using System.Text;

using Server;
using Server.Commands;
using Server.Misc;
#endregion

namespace VitaNex.Modules.Voting
{
	public static class VoteGumpUtility
	{
		public static StringBuilder GetHelpText(Mobile m)
		{
			var help = new StringBuilder();

			help.AppendFormat("<basefont color=#{0:X6}>", Color.SkyBlue.ToRgb());
			help.AppendLine(
				"The Voting service allows you to vote for " + ServerList.ServerName + " and receive rewards (usually tokens).");

			if (Voting.CMOptions.DailyLimit > 0)
			{
				help.AppendLine();
				help.AppendFormat("<basefont color=#{0:X6}>", Color.Orange.ToRgb());
				help.AppendLine(
					String.Format(
						"There is a daily limit of {0:#,0} tokens, when you reach this limit you can still vote, but will not receive any tokens.",
						Voting.CMOptions.DailyLimit));
			}

			help.AppendLine();
			help.AppendFormat("<basefont color=#{0:X6}>", Color.Yellow.ToRgb());
			help.AppendLine("All successful votes will be logged to your personal vote profile.");
			help.AppendLine("These logs are separated by day and can be viewed at any time.");

			if (!String.IsNullOrWhiteSpace(Voting.CMOptions.ProfilesCommand))
			{
				help.AppendLine();
				help.AppendFormat("<basefont color=#{0:X6}>", Color.YellowGreen.ToRgb());
				help.AppendLine(
					String.Format(
						"To view vote profiles, use the <big>{0}{1}</big> command.",
						CommandSystem.Prefix,
						Voting.CMOptions.ProfilesCommand));

				if (m.AccessLevel >= Voting.Access)
				{
					help.AppendLine("You have access to administrate the voting system, you can also manage profiles.");
				}
			}

			if (m.AccessLevel >= Voting.Access)
			{
				if (!String.IsNullOrWhiteSpace(Voting.CMOptions.AdminCommand))
				{
					help.AppendLine();
					help.AppendFormat("<basefont color=#{0:X6}>", Color.LimeGreen.ToRgb());
					help.AppendLine(
						String.Format(
							"To administrate the voting system, use the <big>{0}{1}</big> command.",
							CommandSystem.Prefix,
							Voting.CMOptions.AdminCommand));
				}
			}

			return help;
		}
	}
}