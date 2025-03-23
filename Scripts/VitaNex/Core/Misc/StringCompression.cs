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
using System.IO;
using System.IO.Compression;
using System.Text;
#endregion

namespace VitaNex
{
	public static class StringCompression
	{
		public static Encoding DefaultEncoding = Encoding.UTF8;

		public static byte[] Pack(string str)
		{
			return Pack(str, DefaultEncoding);
		}

		public static byte[] Pack(string str, Encoding enc)
		{
			var bytes = enc.GetBytes(str);

			using (var stdIn = new MemoryStream(bytes))
			{
				using (var stdOut = new MemoryStream())
				{
					using (var s = new GZipStream(stdOut, CompressionMode.Compress))
					{
						stdIn.CopyTo(s);
					}

					return stdOut.ToArray();
				}
			}
		}

		public static string Unpack(byte[] bytes)
		{
			return Unpack(bytes, DefaultEncoding);
		}

		public static string Unpack(byte[] bytes, Encoding enc)
		{
			using (var stdIn = new MemoryStream(bytes))
			{
				using (var stdOut = new MemoryStream())
				{
					using (var s = new GZipStream(stdIn, CompressionMode.Decompress))
					{
						s.CopyTo(stdOut);
					}

					return enc.GetString(stdOut.ToArray());
				}
			}
		}
	}
}