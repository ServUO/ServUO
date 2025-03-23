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
using System.Net;
#endregion

namespace VitaNex.Web
{
	public delegate void WebAPIContextHandler(WebAPIContext context);

	public delegate void WebAPIClientConnected(WebAPIClient client);

	public delegate void WebAPIClientDisconnected(WebAPIClient client);

	public delegate void WebAPIRequestSend<in T>(HttpWebRequest req, T state);

	public delegate void WebAPIRequestReceive<in T>(HttpWebRequest req, T state, HttpWebResponse res);
}