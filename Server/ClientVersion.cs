#region References
using System;
using System.Collections;
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

	public class ClientVersion : IComparable, IComparer
	{
		private readonly string m_SourceString;

		public int Major { get; }
		public int Minor { get; }
		public int Revision { get; }
		public int Patch { get; }

		public ClientType Type { get; }

		public string SourceString => m_SourceString;

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

			if (Type != ClientType.SA && Major >= 67)
			{
				Type = ClientType.SA;
			}

			m_SourceString = _ToStringImpl();
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

		public override int GetHashCode()
		{
			return Major ^ Minor ^ Revision ^ Patch ^ (int)Type;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			var v = obj as ClientVersion;

			if (v == null)
			{
				return false;
			}

			return Major == v.Major && Minor == v.Minor && Revision == v.Revision && Patch == v.Patch && Type == v.Type;
		}

		private string _ToStringImpl()
		{
			var builder = new StringBuilder(16);

			builder.Append(Major);
			builder.Append('.');
			builder.Append(Minor);
			builder.Append('.');
			builder.Append(Revision);

			if (Major <= 5 && Minor <= 0 && Revision <= 6) //Anything before 5.0.7
			{
				if (Patch > 0)
				{
					builder.Append((char)('a' + (Patch - 1)));
				}
			}
			else
			{
				builder.Append('.');
				builder.Append(Patch);
			}

			if (Type != ClientType.Regular)
			{
				builder.Append(' ');
				builder.Append(Type.ToString());
			}

			return builder.ToString();
		}

		public override string ToString()
		{
			return _ToStringImpl();
		}

		public ClientVersion(string fmt)
		{
			m_SourceString = fmt;

			try
			{
				fmt = fmt.ToLower();

				var br1 = fmt.IndexOf('.');
				var br2 = fmt.IndexOf('.', br1 + 1);

				var br3 = br2 + 1;
				while (br3 < fmt.Length && Char.IsDigit(fmt, br3))
				{
					br3++;
				}

				Major = Utility.ToInt32(fmt.Substring(0, br1));
				Minor = Utility.ToInt32(fmt.Substring(br1 + 1, br2 - br1 - 1));
				Revision = Utility.ToInt32(fmt.Substring(br2 + 1, br3 - br2 - 1));

				if (br3 < fmt.Length)
				{
					if (Major <= 5 && Minor <= 0 && Revision <= 6) //Anything before 5.0.7
					{
						if (!Char.IsWhiteSpace(fmt, br3))
						{
							Patch = fmt[br3] - 'a' + 1;
						}
					}
					else
					{
						Patch = Utility.ToInt32(fmt.Substring(br3 + 1, fmt.Length - br3 - 1));
					}
				}

				if (Major >= 6 && Revision >= 14 && Patch >= 3)
				{
					Type = ClientType.SA;
				}
				else if (fmt.IndexOf("god") >= 0 || fmt.IndexOf("gq") >= 0)
				{
					Type = ClientType.God;
				}
				else if (fmt.IndexOf("third dawn") >= 0 || fmt.IndexOf("uo:td") >= 0 || fmt.IndexOf("uotd") >= 0 ||
						 fmt.IndexOf("uo3d") >= 0 || fmt.IndexOf("uo:3d") >= 0)
				{
					Type = ClientType.UOTD;
				}
				else
				{
					Type = ClientType.Regular;
				}
			}
			catch
			{
				Major = 0;
				Minor = 0;
				Revision = 0;
				Patch = 0;
				Type = ClientType.Regular;
			}
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}

			var o = obj as ClientVersion;

			if (o == null)
			{
				throw new ArgumentException();
			}

			if (Major > o.Major)
			{
				return 1;
			}
			else if (Major < o.Major)
			{
				return -1;
			}
			else if (Minor > o.Minor)
			{
				return 1;
			}
			else if (Minor < o.Minor)
			{
				return -1;
			}
			else if (Revision > o.Revision)
			{
				return 1;
			}
			else if (Revision < o.Revision)
			{
				return -1;
			}
			else if (Patch > o.Patch)
			{
				return 1;
			}
			else if (Patch < o.Patch)
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}

		public static bool IsNull(object x)
		{
			return ReferenceEquals(x, null);
		}

		public int Compare(object x, object y)
		{
			if (IsNull(x) && IsNull(y))
			{
				return 0;
			}
			else if (IsNull(x))
			{
				return -1;
			}
			else if (IsNull(y))
			{
				return 1;
			}

			var a = x as ClientVersion;
			var b = y as ClientVersion;

			if (IsNull(a) || IsNull(b))
			{
				throw new ArgumentException();
			}

			return a.CompareTo(b);
		}

		public static int Compare(ClientVersion a, ClientVersion b)
		{
			if (IsNull(a) && IsNull(b))
			{
				return 0;
			}
			else if (IsNull(a))
			{
				return -1;
			}
			else if (IsNull(b))
			{
				return 1;
			}

			return a.CompareTo(b);
		}
	}
}