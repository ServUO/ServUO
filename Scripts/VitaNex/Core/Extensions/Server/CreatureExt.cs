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
using Server.Mobiles;
#endregion

namespace Server
{
	public static class CreatureExtUtility
	{
		public static TMobile GetMaster<TMobile>(this BaseCreature creature)
			where TMobile : Mobile
		{
			return creature != null ? creature.GetMaster() as TMobile : null;
		}
	}
}