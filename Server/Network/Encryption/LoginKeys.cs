#region References
using System;
using System.Collections.Generic;
#endregion

namespace Server.Network.Encryption
{
	public readonly struct LoginKeys : IComparable, IComparable<LoginKeys>, IEquatable<LoginKeys>
	{
		public static readonly LoginKeys Empty = new LoginKeys(ClientVersion.Zero, 0, 0);

		private static readonly SortedDictionary<ClientVersion, LoginKeys> m_Table = new SortedDictionary<ClientVersion, LoginKeys>();

		public static LoginKeys Acquire(ClientVersion ver)
		{
			if (!m_Table.TryGetValue(ver, out var key))
			{
				var major = (uint)ver.Major;
				var minor = (uint)ver.Minor;
				var revision = (uint)ver.Revision;

				uint key1, key2;

				key1 = (major << 23) | (minor << 14) | (revision << 4);
				key1 ^= (revision * revision) << 9;
				key1 ^= (minor * minor);
				key1 ^= (minor * 11) << 24;
				key1 ^= (revision * 7) << 19;
				key1 ^= 0x2C13A5FD;

				key2 = (major << 22) | (revision << 13) | (minor << 3);
				key2 ^= (revision * revision * 3) << 10;
				key2 ^= (minor * minor);
				key2 ^= (minor * 13) << 23;
				key2 ^= (revision * 7) << 18;
				key2 ^= 0xA31D527F;

				m_Table[ver] = key = new LoginKeys(ver, key1, key2);
			}

			return key;
		}

		public static int Compare(LoginKeys a, LoginKeys b)
		{
			return a.CompareTo(b);
		}

		public readonly int Hash;
		public readonly string SourceString;

		public readonly ClientVersion Version;

		public readonly uint Key1, Key2;

		public LoginKeys(ClientVersion ver, uint key1, uint key2)
		{
			Hash = unchecked((int)(key1 ^ key2));
			SourceString = $"{ver}: 0x{key1:X}, 0x{key2:X}";

			Version = ver;

			Key1 = key1;
			Key2 = key2;
		}

		public override int GetHashCode()
		{
			return Hash;
		}

		public override string ToString()
		{
			return SourceString;
		}

		public override bool Equals(object obj)
		{
			return obj is LoginKeys keys && Equals(keys);
		}

		public bool Equals(LoginKeys keys)
		{
			return keys.Hash == Hash;
		}

		public int CompareTo(LoginKeys keys)
		{
			return Version.CompareTo(keys.Version);
		}

		int IComparable.CompareTo(object obj)
		{
			if (obj is LoginKeys keys)
			{
				return CompareTo(keys);
			}

			throw new ArgumentException("The given type is invalid for this comparison", nameof(obj));
		}

		public static bool operator ==(LoginKeys l, LoginKeys r)
		{
			return Compare(l, r) == 0;
		}

		public static bool operator !=(LoginKeys l, LoginKeys r)
		{
			return Compare(l, r) != 0;
		}

		public static bool operator >=(LoginKeys l, LoginKeys r)
		{
			return Compare(l, r) >= 0;
		}

		public static bool operator >(LoginKeys l, LoginKeys r)
		{
			return Compare(l, r) > 0;
		}

		public static bool operator <=(LoginKeys l, LoginKeys r)
		{
			return Compare(l, r) <= 0;
		}

		public static bool operator <(LoginKeys l, LoginKeys r)
		{
			return Compare(l, r) < 0;
		}
	}
}
