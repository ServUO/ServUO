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
using System.Text;

using VitaNex.IO;
#endregion

namespace VitaNex.Web
{
	public class WebAPIRequest : IDisposable
	{
		public WebAPIClient Client { get; private set; }

		public WebAPIHeaders Headers { get; private set; }
		public WebAPIQueries Queries { get; private set; }

		public FileMime ContentType { get; set; }
		public Encoding Encoding { get; set; }
		public string Data { get; set; }
		public int Length { get; set; }

		public WebAPIRequest(WebAPIClient client)
		{
			Client = client;

			Headers = new WebAPIHeaders();
			Queries = new WebAPIQueries();

			ContentType = FileMime.Default;
			Encoding = Encoding.UTF8;

			Data = String.Empty;
			Length = 0;
		}

		public void Dispose()
		{
			Headers.Dispose();
			Headers = null;

			Queries.Dispose();
			Queries = null;

			Encoding = null;

			Data = null;
			Length = 0;

			Client = null;
		}
	}
}