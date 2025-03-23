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
using System.Net;
using System.Text;

using VitaNex.IO;
#endregion

namespace VitaNex.Web
{
	public class WebAPIResponse : IDisposable
	{
		public WebAPIClient Client { get; private set; }
		public WebAPIHeaders Headers { get; private set; }

		public HttpStatusCode Status { get; set; }
		public FileMime ContentType { get; set; }
		public Encoding Encoding { get; set; }
		public object Data { get; set; }

		public bool Compress { get; set; }
		public bool FreeData { get; set; }

		public string FileName { get; set; }
		public int Cache { get; set; }

		public WebAPIResponse(WebAPIClient client)
		{
			Client = client;

			Headers = new WebAPIHeaders();

			Compress = false;
			FreeData = true;

			Cache = -1;
			FileName = String.Empty;
			Encoding = Encoding.UTF8;

			Status = HttpStatusCode.OK;
			ContentType = FileMime.Default;
		}

		public void Dispose()
		{
			if (FreeData && Data is IDisposable)
			{
				((IDisposable)Data).Dispose();
			}

			Data = null;

			Headers.Dispose();
			Headers = null;

			Cache = -1;
			FileName = null;
			Encoding = null;

			Client = null;
		}
	}
}