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
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
#endregion

namespace VitaNex.Crypto
{
	/// <summary>
	///     Jenkins Lookup 3 non-cryptographic HashAlgorithm implementation.
	///     Reference: https://en.wikipedia.org/wiki/Jenkins_hash_function
	/// </summary>
	[ComVisible(true)]
	public abstract class Jenkins3 : HashAlgorithm
	{
		private static readonly string[] _Names = { "J3", "Jenkins3", "Jenkins-3", "VitaNex.Crypto.Jenkins3" };

		static Jenkins3()
		{
			CryptoConfig.AddAlgorithm(typeof(Jenkins3CryptoServiceProvider), _Names);
		}

		[SecuritySafeCritical]
		public static new Jenkins3 Create()
		{
			return Create("VitaNex.Crypto.Jenkins3");
		}

		[SecuritySafeCritical]
		public static new Jenkins3 Create(string algName)
		{
			return (Jenkins3)CryptoConfig.CreateFromName(algName);
		}

		protected Jenkins3()
		{
			HashSizeValue = 64;
		}
	}

	/// <summary>
	///     Jenkins Lookup 3 non-cryptographic HashAlgorithm implementation.
	///     Reference: https://en.wikipedia.org/wiki/Jenkins_hash_function
	/// </summary>
	public sealed class Jenkins3CryptoServiceProvider : Jenkins3
	{
		public const uint DeadBeef = 0xDEADBEEF;

		private string _Seed;
		private uint _X0, _X1, _X2, _X3, _X4, _X5;

		public override void Initialize()
		{
			_Seed = String.Empty;
			_X0 = _X1 = _X2 = _X3 = _X4 = _X5 = 0;
		}

		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			if (ibStart == 0 && cbSize == array.Length)
			{
				_Seed += BitConverter.ToString(array);
			}
			else
			{
				_Seed += BitConverter.ToString(array, ibStart, cbSize);
			}
		}

		protected override byte[] HashFinal()
		{
			_X0 = _X1 = _X2 = DeadBeef + (uint)_Seed.Length;
			_X3 = _X4 = 0;

			int pos;

			for (pos = 0; pos + 12 < _Seed.Length; pos += 12)
			{
				_X1 = (uint)((_Seed[pos + 7] << 24) | (_Seed[pos + 6] << 16) | (_Seed[pos + 5] << 8) | _Seed[pos + 4]) + _X1;
				_X2 = (uint)((_Seed[pos + 11] << 24) | (_Seed[pos + 10] << 16) | (_Seed[pos + 9] << 8) | _Seed[pos + 8]) + _X2;
				_X3 = (uint)((_Seed[pos + 3] << 24) | (_Seed[pos + 2] << 16) | (_Seed[pos + 1] << 8) | _Seed[pos]) - _X2;

				_X3 = (_X3 + _X0) ^ (_X2 >> 28) ^ (_X2 << 4);
				_X2 += _X1;
				_X1 = (_X1 - _X3) ^ (_X3 >> 26) ^ (_X3 << 6);
				_X3 += _X2;
				_X2 = (_X2 - _X1) ^ (_X1 >> 24) ^ (_X1 << 8);
				_X1 += _X3;
				_X0 = (_X3 - _X2) ^ (_X2 >> 16) ^ (_X2 << 16);
				_X2 += _X1;
				_X1 = (_X1 - _X0) ^ (_X0 >> 13) ^ (_X0 << 19);
				_X0 += _X2;
				_X2 = (_X2 - _X1) ^ (_X1 >> 28) ^ (_X1 << 4);
				_X1 += _X0;
			}

			var rem = _Seed.Length - pos;

			if (rem <= 0)
			{
				return BitConverter.GetBytes(((ulong)_X2 << 32) | _X4);
			}

			switch (rem)
			{
				case 12:
					_X2 += (uint)_Seed[pos + 11] << 24;
					goto case 11;
				case 11:
					_X2 += (uint)_Seed[pos + 10] << 16;
					goto case 10;
				case 10:
					_X2 += (uint)_Seed[pos + 9] << 8;
					goto case 9;
				case 9:
					_X2 += _Seed[pos + 8];
					goto case 8;
				case 8:
					_X1 += (uint)_Seed[pos + 7] << 24;
					goto case 7;
				case 7:
					_X1 += (uint)_Seed[pos + 6] << 16;
					goto case 6;
				case 6:
					_X1 += (uint)_Seed[pos + 5] << 8;
					goto case 5;
				case 5:
					_X1 += _Seed[pos + 4];
					goto case 4;
				case 4:
					_X0 += (uint)_Seed[pos + 3] << 24;
					goto case 3;
				case 3:
					_X0 += (uint)_Seed[pos + 2] << 16;
					goto case 2;
				case 2:
					_X0 += (uint)_Seed[pos + 1] << 8;
					goto case 1;
				case 1:
					_X0 += _Seed[pos];
					break;
			}

			_X2 = (_X2 ^ _X1) - ((_X1 >> 18) ^ (_X1 << 14));
			_X5 = (_X2 ^ _X0) - ((_X2 >> 21) ^ (_X2 << 11));
			_X1 = (_X1 ^ _X5) - ((_X5 >> 7) ^ (_X5 << 25));
			_X2 = (_X2 ^ _X1) - ((_X1 >> 16) ^ (_X1 << 16));
			_X3 = (_X2 ^ _X5) - ((_X2 >> 28) ^ (_X2 << 4));
			_X1 = (_X1 ^ _X3) - ((_X3 >> 18) ^ (_X3 << 14));
			_X4 = (_X2 ^ _X1) - ((_X1 >> 8) ^ (_X1 << 24));

			return BitConverter.GetBytes(((ulong)_X1 << 32) | _X4);
		}
	}
}