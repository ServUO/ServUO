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

namespace VitaNex.Crypto
{
	public static class CryptoGenerator
	{
		public static string GenString(CryptoHashType type, string seed)
		{
			var p = CryptoService.GetProvider(type);

			return p != null ? p.Generate(seed) : String.Empty;
		}

		public static string GenString(int type, string seed)
		{
			var p = CryptoService.GetProvider(type);

			return p != null ? p.Generate(seed) : String.Empty;
		}

		public static CryptoHashCode GenHashCode(CryptoHashType type, string seed)
		{
			return new CryptoHashCode((int)type, seed);
		}

		public static CryptoHashCode GenHashCode(int type, string seed)
		{
			return new CryptoHashCode(type, seed);
		}

		public static CryptoHashCodeTable GenTable(string seed)
		{
			return new CryptoHashCodeTable(seed);
		}
	}
}