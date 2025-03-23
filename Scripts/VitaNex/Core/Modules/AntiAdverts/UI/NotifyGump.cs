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

using VitaNex.Notify;
#endregion

namespace VitaNex.Modules.AntiAdverts
{
	public sealed class AntiAdvertNotifyGump : NotifyGump
	{
		private static void InitSettings(NotifySettings settings)
		{
			settings.CanIgnore = true;
			settings.Access = AntiAdverts.Access;
			settings.Desc = "Advertising Reports";
		}

		public AntiAdvertNotifyGump(Mobile user, string html)
			: base(user, html)
		{ }
	}
}