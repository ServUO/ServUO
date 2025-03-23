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

using Server.Mobiles;
#endregion

namespace VitaNex.Modules.CastBars
{
	public sealed class CastBarRequestEventArgs : EventArgs
	{
		public PlayerMobile User { get; private set; }
		public Point Location { get; set; }
		public SpellCastBar Gump { get; set; }

		public CastBarRequestEventArgs(PlayerMobile user, Point loc)
		{
			User = user;
			Location = loc;
		}
	}
}