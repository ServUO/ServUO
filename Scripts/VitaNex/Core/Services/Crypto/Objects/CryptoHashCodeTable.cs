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
#endregion

namespace VitaNex.Crypto
{
	public class CryptoHashCodeTable : List<CryptoHashCode>, IEquatable<CryptoHashCodeTable>, IDisposable
	{
		public bool IsDisposed { get; private set; }

		public CryptoHashCodeTable(string seed)
		{
			AddRange(CryptoService.Providers.Keys.Select(type => CryptoGenerator.GenHashCode(type, seed)));
		}

		~CryptoHashCodeTable()
		{
			Dispose();
		}

		public virtual void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}

			IsDisposed = true;

			GC.SuppressFinalize(this);

			this.Free(true);
		}

		public override int GetHashCode()
		{
			return this.GetContentsHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is CryptoHashCodeTable && Equals((CryptoHashCodeTable)obj);
		}

		public virtual bool Equals(CryptoHashCodeTable table)
		{
			return !ReferenceEquals(table, null) && (ReferenceEquals(table, this) || this.ContentsEqual(table));
		}

		public override string ToString()
		{
			return String.Join("+", this);
		}

		public static bool operator ==(CryptoHashCodeTable l, CryptoHashCodeTable r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(CryptoHashCodeTable l, CryptoHashCodeTable r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}
	}
}