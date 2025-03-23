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
using System.Security.Cryptography;
using System.Text;
#endregion

namespace VitaNex.Crypto
{
	public sealed class CryptoHashCodeProvider : ICryptoProvider
	{
		public bool IsDisposed { get; private set; }

		public int ProviderID { get; private set; }
		public HashAlgorithm Provider { get; private set; }

		public string Seed { get; private set; }
		public byte[] Buffer { get; private set; }

		public CryptoHashCodeProvider(int id, HashAlgorithm hal)
		{
			ProviderID = id;
			Provider = hal;
		}

		~CryptoHashCodeProvider()
		{
			Dispose();
		}

		public override int GetHashCode()
		{
			return ProviderID;
		}

		public override bool Equals(object obj)
		{
			return obj is CryptoHashCodeProvider && Equals((CryptoHashCodeProvider)obj);
		}

		public override string ToString()
		{
			return String.Format("{0}:{1}", ProviderID, Provider != null ? Provider.GetType().FullName : "NULL");
		}

		public string Generate(string seed)
		{
			seed = seed ?? String.Empty;

			if (Buffer == null || Seed != seed)
			{
				Seed = seed;

				Buffer = Encoding.UTF32.GetBytes(Seed);
				Buffer = Provider.ComputeHash(Buffer);
			}

			return BitConverter.ToString(Buffer);
		}

		public void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}

			IsDisposed = true;

			GC.SuppressFinalize(this);

			if (Provider != null)
			{
				VitaNexCore.TryCatch(Provider.Clear);
			}

			Provider = null;
			Seed = null;
			Buffer = null;
		}

		public static bool operator ==(CryptoHashCodeProvider l, CryptoHashCodeProvider r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(CryptoHashCodeProvider l, CryptoHashCodeProvider r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}
	}
}