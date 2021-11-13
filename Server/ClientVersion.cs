#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Server
{
	public enum ClientType
	{
		Regular,
		UOTD,
		God,
		SA
	}

	public struct ClientVersion : IComparable, IComparable<ClientVersion>, IEquatable<ClientVersion>
	{
		public static readonly ClientVersion Zero = new ClientVersion(0, 0, 0, 0);

		public static bool TryParse(string input, out int maj, out int min, out int rev, out int pat, out ClientType type)
		{
			try
			{
				var fmt = input.ToLower();

				var br1 = fmt.IndexOf('.');
				var br2 = fmt.IndexOf('.', br1 + 1);

				var br3 = br2 + 1;

				while (br3 < fmt.Length && Char.IsDigit(fmt, br3))
				{
					br3++;
				}

				maj = Int32.Parse(fmt.Substring(0, br1));
				min = Int32.Parse(fmt.Substring(br1 + 1, br2 - br1 - 1));
				rev = Int32.Parse(fmt.Substring(br2 + 1, br3 - br2 - 1));
				pat = 0;
				type = ClientType.Regular;

				if (br3 < fmt.Length)
				{
					if (maj <= 5 && min <= 0 && rev <= 6) //Anything before 5.0.7
					{
						if (!Char.IsWhiteSpace(fmt, br3))
						{
							pat = fmt[br3] - 'a' + 1;
						}
					}
					else
					{
						pat = Int32.Parse(fmt.Substring(br3 + 1, fmt.Length - br3 - 1));
					}
				}

				if (maj >= 6 && rev >= 14 && pat >= 3)
				{
					type = ClientType.SA;
				}
				else if (Insensitive.Contains(fmt, "god") || Insensitive.Contains(fmt, "gq"))
				{
					type = ClientType.God;
				}
				else if (Insensitive.Contains(fmt, "third dawn") || Insensitive.Contains(fmt, "uo:td") || Insensitive.Contains(fmt, "uotd") || Insensitive.Contains(fmt, "uo3d") || Insensitive.Contains(fmt, "uo:3d"))
				{
					type = ClientType.UOTD;
				}
				else
				{
					type = ClientType.Regular;
				}

				if (maj >= 67 && type != ClientType.SA)
				{
					type = ClientType.SA;
				}

				return true;
			}
			catch
			{
				maj = min = rev = pat = 0;
				type = ClientType.Regular;

				return false;
			}
		}

		public static bool TryParse(string input, out ClientVersion value)
		{
			value = Zero;

			if (!TryParse(input, out var maj, out var min, out var rev, out var pat, out var type))
			{
				return false;
			}

			if (maj != 0 || min != 0 || rev != 0 || pat != 0 || type != ClientType.Regular)
			{
				value = new ClientVersion(maj, min, rev, pat, type);
			}

			return true;
		}

		public static int Compare(ClientVersion a, ClientVersion b)
		{
			return a.CompareTo(b);
		}

		public readonly int Hash;

		public readonly string SourceString;

		public readonly int Major, Minor, Revision, Patch;

		public readonly ClientType Type;

		public ClientVersion(int maj, int min, int rev, int pat)
			: this(maj, min, rev, pat, ClientType.Regular)
		{ }

		public ClientVersion(int maj, int min, int rev, int pat, ClientType type)
		{
			Major = maj;
			Minor = min;
			Revision = rev;
			Patch = pat;
			Type = type;

			if (Major >= 67 && Type != ClientType.SA)
			{
				Type = ClientType.SA;
			}

			Hash = 0;
			SourceString = null;

			Hash = GetHash();
			SourceString = GetString();
		}

		public ClientVersion(string version)
		{
			Hash = 0;
			SourceString = version;

			_ = TryParse(SourceString, out Major, out Minor, out Revision, out Patch, out Type);

			Hash = GetHash();
			SourceString = GetString();
		}

		private int GetHash()
		{
			unchecked
			{
				var hash = (int)Type;

				hash = (hash * 397) ^ Major;
				hash = (hash * 397) ^ Minor;
				hash = (hash * 397) ^ Revision;
				hash = (hash * 397) ^ Patch;

				return hash;
			}
		}

		private string GetString()
		{
			var str = $"{Major}.{Minor}.{Revision}";

			if (Major > 5 || Minor > 0 || Revision > 6)
			{
				str = $"{str}.{Patch}";
			}
			else if (Patch > 0)
			{
				str = $"{str}.{(char)('a' + (Patch - 1))}";
			}

			if (Type != ClientType.Regular)
			{
				str = $"{str} {Type}";
			}

			return String.Intern(str);
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
			return obj is ClientVersion v && Equals(v);
		}

		public bool Equals(ClientVersion v)
		{
			return Major == v.Major && Minor == v.Minor && Revision == v.Revision && Patch == v.Patch && Type == v.Type;
		}

		public int CompareTo(ClientVersion v)
		{
			if (Major > v.Major)
			{
				return 1;
			}

			if (Major < v.Major)
			{
				return -1;
			}

			if (Minor > v.Minor)
			{
				return 1;
			}

			if (Minor < v.Minor)
			{
				return -1;
			}

			if (Revision > v.Revision)
			{
				return 1;
			}

			if (Revision < v.Revision)
			{
				return -1;
			}

			if (Patch > v.Patch)
			{
				return 1;
			}

			if (Patch < v.Patch)
			{
				return -1;
			}

			return 0;
		}

		int IComparable.CompareTo(object obj)
		{
			if (obj is ClientVersion v)
			{
				return CompareTo(v);
			}

			throw new ArgumentException("The given type is invalid for this comparison", nameof(obj));
		}

		public static bool operator ==(ClientVersion l, ClientVersion r)
		{
			return Compare(l, r) == 0;
		}

		public static bool operator !=(ClientVersion l, ClientVersion r)
		{
			return Compare(l, r) != 0;
		}

		public static bool operator >=(ClientVersion l, ClientVersion r)
		{
			return Compare(l, r) >= 0;
		}

		public static bool operator >(ClientVersion l, ClientVersion r)
		{
			return Compare(l, r) > 0;
		}

		public static bool operator <=(ClientVersion l, ClientVersion r)
		{
			return Compare(l, r) <= 0;
		}

		public static bool operator <(ClientVersion l, ClientVersion r)
		{
			return Compare(l, r) < 0;
		}
	}
}
