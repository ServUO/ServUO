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
#endregion

namespace VitaNex.Web
{
	public class WebAPIContext : IDisposable
	{
		public WebAPIClient Client { get; private set; }

		public WebAPIMethod Method { get; private set; }
		public string Uri { get; private set; }

		public WebAPIRequest Request { get; private set; }
		public WebAPIResponse Response { get; private set; }

		public bool Authorized { get; set; }

		public WebAPIContext(WebAPIClient client, WebAPIMethod method, string uri)
		{
			Client = client;

			Method = method;
			Uri = uri;

			Request = new WebAPIRequest(Client);
			Response = new WebAPIResponse(Client);
		}

		public void Dispose()
		{
			Client = null;

			Method = WebAPIMethod.UNKNOWN;
			Uri = null;

			Request.Dispose();
			Request = null;

			Response.Dispose();
			Response = null;
		}
	}
}