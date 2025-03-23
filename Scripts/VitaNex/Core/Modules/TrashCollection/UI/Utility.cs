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
#endregion

namespace VitaNex.Modules.TrashCollection
{
	public static class TrashCollectionGumpUtility
	{
		public static StringBuilder GetHelpText(Mobile m)
		{
			var help = new StringBuilder();

			help.AppendFormat("<basefont color=#{0:X6}>", Color.SkyBlue.ToRgb());
			help.AppendLine("The Trash Collection service allows you to trash items in exchange for currency (usually tokens).");

			if (TrashCollection.CMOptions.DailyLimit > 0)
			{
				help.AppendLine();
				help.AppendFormat("<basefont color=#{0:X6}>", Color.Orange.ToRgb());
				help.AppendLine(
					String.Format(
						"There is a daily limit of {0:#,0} tokens, when you reach this limit you can still trash items, but will not receive any tokens.",
						TrashCollection.CMOptions.DailyLimit));
			}

			help.AppendLine();
			help.AppendFormat("<basefont color=#{0:X6}>", Color.Yellow.ToRgb());
			help.AppendLine("Everything you successfully trash will be logged to your personal trash profile.");
			help.AppendLine("These logs are separated by day and can be viewed at any time.");

			if (!String.IsNullOrWhiteSpace(TrashCollection.CMOptions.ProfilesCommand))
			{
				help.AppendLine();
				help.AppendFormat("<basefont color=#{0:X6}>", Color.YellowGreen.ToRgb());
				help.AppendLine(
					String.Format(
						"To view trash profiles, use the <big>{0}{1}</big> command.",
						CommandSystem.Prefix,
						TrashCollection.CMOptions.ProfilesCommand));

				if (m.AccessLevel >= TrashCollection.Access)
				{
					help.AppendLine("You have access to administrate the trash system, you can also manage profiles.");
				}
			}

			if (m.AccessLevel >= TrashCollection.Access)
			{
				if (!String.IsNullOrWhiteSpace(TrashCollection.CMOptions.AdminCommand))
				{
					help.AppendLine();
					help.AppendFormat("<basefont color=#{0:X6}>", Color.LimeGreen.ToRgb());
					help.AppendLine(
						String.Format(
							"To administrate the trash system, use the <big>{0}{1}</big> command.",
							CommandSystem.Prefix,
							TrashCollection.CMOptions.AdminCommand));
				}
			}

			return help;
		}
	}
}