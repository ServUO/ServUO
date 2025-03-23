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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server;
#endregion

namespace VitaNex.Crypto
{
	public class CryptoHashCode : IEquatable<CryptoHashCode>, IComparable<CryptoHashCode>, IEnumerable<char>, IDisposable
	{
		private int _ValueHash = -1;

		public int ValueHash => GetValueHash();

		public bool IsDisposed { get; private set; }

		public bool IsExtended => CryptoService.IsExtended(ProviderID);

		public int ProviderID { get; protected set; }

		public virtual string Value { get; private set; }

		public int Length => Value.Length;

		public char this[int index] => Value[index];

		public CryptoHashCode(CryptoHashType type, string seed)
			: this((int)type, seed)
		{ }

		public CryptoHashCode(int providerID, string seed)
		{
			ProviderID = providerID;

			Value = CryptoGenerator.GenString(ProviderID, seed ?? String.Empty);
		}

		public CryptoHashCode(GenericReader reader)
		{
			Deserialize(reader);
		}

		~CryptoHashCode()
		{
			Dispose();
		}

		public override string ToString()
		{
			return Value;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<char> GetEnumerator()
		{
			return Value.GetEnumerator();
		}

		public virtual int CompareTo(CryptoHashCode code)
		{
			return !ReferenceEquals(code, null) ? Insensitive.Compare(Value, code.Value) : -1;
		}

		public override bool Equals(object obj)
		{
			return obj is CryptoHashCode && Equals((CryptoHashCode)obj);
		}

		public bool Equals(CryptoHashCode other)
		{
			return !ReferenceEquals(other, null) && ValueHash == other.ValueHash;
		}

		public override int GetHashCode()
		{
			return unchecked(((ProviderID + 1) * 397) ^ ValueHash);
		}

		public int GetValueHash()
		{
			if (_ValueHash > -1)
			{
				return _ValueHash;
			}

			var hash = Value.Aggregate(Value.Length, (h, c) => unchecked((h * 397) ^ c));

			// It may be negative, so ensure it is positive, normally this wouldn't be the case but negative integers for 
			// almost unique id's should be positive for things like database keys.
			// Note this increases chance of unique collisions by 50%, though still extremely unlikely;
			// 1 : 2,147,483,647
			return _ValueHash = Math.Abs(hash);
		}

		public virtual void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}

			IsDisposed = true;

			GC.SuppressFinalize(this);

			Value = null;
		}

		public virtual void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(2);

			writer.Write(ProviderID);

			switch (version)
			{
				case 2:
				{
					writer.Write(Value);
					writer.Write(_ValueHash);
				}
				break;
				case 1:
				case 0:
					break;
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			ProviderID = reader.ReadInt();

			switch (version)
			{
				case 2:
				{
					Value = reader.ReadString();
					_ValueHash = reader.ReadInt();
				}
				break;
				case 1:
				{
					var seed = reader.ReadBool()
						? StringCompression.Unpack(reader.ReadBytes())
						: Encoding.UTF32.GetString(reader.ReadBytes());

					Value = CryptoGenerator.GenString(ProviderID, seed ?? String.Empty);
				}
				break;
				case 0:
				{
					var seed = reader.ReadString();

					Value = CryptoGenerator.GenString(ProviderID, seed ?? String.Empty);
				}
				break;
			}
		}

		public static bool operator ==(CryptoHashCode l, CryptoHashCode r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(CryptoHashCode l, CryptoHashCode r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}
	}
}