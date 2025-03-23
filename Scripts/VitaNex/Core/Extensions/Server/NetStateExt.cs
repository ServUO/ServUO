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

#if ServUO58
#define ServUOX
#endif

#region References
#if !ServUOX
using System;
#endif
#endregion

namespace Server.Network
{
	public static class NetStateExtUtility
	{
		private static readonly ClientVersion _70500 = new ClientVersion(7, 0, 50, 0);
		private static readonly ClientVersion _70610 = new ClientVersion(7, 0, 61, 0);

		public static bool IsEnhanced(this NetState state)
		{
#if ServUOX
			return state?.IsEnhancedClient == true;
#else
			var v = state.Version;

			if (v == null || (v.Major == 0 && v.Minor == 0 && v.Revision == 0 && v.Patch == 0))
			{
				return false;
			}

			if (!state.GetPropertyValue("IsEnhancedClient", out bool ec))
			{
				ec = v.Major >= 67 || v.Type == ClientType.UOTD;
			}

			return ec;
#endif
		}

		public static bool SupportsUltimaStore(this NetState state)
		{
			return state != null && state.Version >= _70500;
		}

		public static bool SupportsEndlessJourney(this NetState state)
		{
			return state != null && state.Version >= _70610;
		}
	}
}
