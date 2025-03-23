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
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
#endregion

namespace VitaNex.Crypto
{
	public static partial class CryptoService
	{
		public static CryptoHashType[] HashTypes { get; private set; }

		public static Dictionary<int, CryptoHashCodeProvider> Providers { get; private set; }

		public static void RegisterProvider(CryptoHashType type, HashAlgorithm hal)
		{
			Providers[(int)type] = new CryptoHashCodeProvider((int)type, hal);
		}

		public static void RegisterProvider(int type, HashAlgorithm hal)
		{
			Providers[type] = new CryptoHashCodeProvider(type, hal);
		}

		public static CryptoHashCodeProvider GetProvider(CryptoHashType type)
		{
			return Providers.GetValue((int)type);
		}

		public static CryptoHashCodeProvider GetProvider(int type)
		{
			return Providers.GetValue(type);
		}

		public static bool IsExtended(int type)
		{
			return HashTypes.Cast<int>().All(t => t != type);
		}
	}
}