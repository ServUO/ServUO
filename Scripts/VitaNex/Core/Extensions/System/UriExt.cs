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
using System.Collections.Generic;

using VitaNex.Web;
#endregion

namespace System
{
	public static class UriExtUtility
	{
		public static IEnumerable<KeyValuePair<string, string>> DecodeQueryString(this Uri uri)
		{
			return WebAPI.DecodeQuery(uri.Query);
		}

		public static Uri EncodeQueryString(this Uri uri, IEnumerable<KeyValuePair<string, string>> queries)
		{
			return new Uri(uri.GetLeftPart(UriPartial.Path) + WebAPI.EncodeQuery(queries));
		}
	}
}