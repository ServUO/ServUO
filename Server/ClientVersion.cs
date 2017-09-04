#region Header
// **********
// ServUO - ClientVersion.cs
// **********
#endregion

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
		private readonly int m_Major;
		private readonly int m_Minor;
		private readonly int m_Revision;
		private readonly int m_Patch;
		private readonly ClientType m_Type;
		private readonly string m_SourceString;

		public int Major { get { return m_Major; } }

		public int Minor { get { return m_Minor; } }

		public int Revision { get { return m_Revision; } }

		public int Patch { get { return m_Patch; } }

		public ClientType Type { get { return m_Type; } }

		public string SourceString { get { return m_SourceString; } }

		public ClientVersion(int maj, int min, int rev, int pat)
			: this(maj, min, rev, pat, ClientType.Regular)
		{ }

		public ClientVersion(int maj, int min, int rev, int pat, ClientType type)
		{
			m_Major = maj;
			m_Minor = min;
			m_Revision = rev;
			m_Patch = pat;
			m_Type = type;

            if (m_Type != ClientType.SA && m_Major >= 67)
            {
                m_Type = ClientType.SA;
            }

			m_SourceString = _ToStringImpl();
		}

		public static bool operator ==(ClientVersion l, ClientVersion r)
		{
			return (Compare(l, r) == 0);
		}

		public static bool operator !=(ClientVersion l, ClientVersion r)
		{
			return (Compare(l, r) != 0);
		}

		public static bool operator >=(ClientVersion l, ClientVersion r)
		{
			return (Compare(l, r) >= 0);
		}

		public static bool operator >(ClientVersion l, ClientVersion r)
		{
			return (Compare(l, r) > 0);
		}

		public static bool operator <=(ClientVersion l, ClientVersion r)
		{
			return (Compare(l, r) <= 0);
		}

		public static bool operator <(ClientVersion l, ClientVersion r)
		{
			return (Compare(l, r) < 0);
		}

		public override int GetHashCode()
		{
			return m_Major ^ m_Minor ^ m_Revision ^ m_Patch ^ (int)m_Type;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			ClientVersion v = obj as ClientVersion;

			if (v == null)
			{
				return false;
			}

			return m_Major == v.m_Major && m_Minor == v.m_Minor && m_Revision == v.m_Revision && m_Patch == v.m_Patch &&
				   m_Type == v.m_Type;
		}

		private string _ToStringImpl()
		{
			StringBuilder builder = new StringBuilder(16);

			builder.Append(m_Major);
			builder.Append('.');
			builder.Append(m_Minor);
			builder.Append('.');
			builder.Append(m_Revision);

			if (m_Major <= 5 && m_Minor <= 0 && m_Revision <= 6) //Anything before 5.0.7
			{
				if (m_Patch > 0)
				{
					builder.Append((char)('a' + (m_Patch - 1)));
				}
			}
			else
			{
				builder.Append('.');
				builder.Append(m_Patch);
			}

			if (m_Type != ClientType.Regular)
			{
				builder.Append(' ');
				builder.Append(m_Type.ToString());
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

				int br1 = fmt.IndexOf('.');
				int br2 = fmt.IndexOf('.', br1 + 1);

				int br3 = br2 + 1;
				while (br3 < fmt.Length && Char.IsDigit(fmt, br3))
				{
					br3++;
				}

				m_Major = Utility.ToInt32(fmt.Substring(0, br1));
				m_Minor = Utility.ToInt32(fmt.Substring(br1 + 1, br2 - br1 - 1));
				m_Revision = Utility.ToInt32(fmt.Substring(br2 + 1, br3 - br2 - 1));

				if (br3 < fmt.Length)
				{
					if (m_Major <= 5 && m_Minor <= 0 && m_Revision <= 6) //Anything before 5.0.7
					{
						if (!Char.IsWhiteSpace(fmt, br3))
						{
							m_Patch = (fmt[br3] - 'a') + 1;
						}
					}
					else
					{
						m_Patch = Utility.ToInt32(fmt.Substring(br3 + 1, fmt.Length - br3 - 1));
					}
				}

                if (m_Major >= 67)
                {
                    m_Type = ClientType.SA;
                }
                else if(fmt.IndexOf("god") >= 0 || fmt.IndexOf("gq") >= 0)
				{
					m_Type = ClientType.God;
				}
				else if (fmt.IndexOf("third dawn") >= 0 || fmt.IndexOf("uo:td") >= 0 || fmt.IndexOf("uotd") >= 0 ||
						 fmt.IndexOf("uo3d") >= 0 || fmt.IndexOf("uo:3d") >= 0)
				{
					m_Type = ClientType.UOTD;
				}
				else
				{
					m_Type = ClientType.Regular;
				}
			}
			catch
			{
				m_Major = 0;
				m_Minor = 0;
				m_Revision = 0;
				m_Patch = 0;
				m_Type = ClientType.Regular;
			}
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}

			ClientVersion o = obj as ClientVersion;

			if (o == null)
			{
				throw new ArgumentException();
			}

			if (m_Major > o.m_Major)
			{
				return 1;
			}
			else if (m_Major < o.m_Major)
			{
				return -1;
			}
			else if (m_Minor > o.m_Minor)
			{
				return 1;
			}
			else if (m_Minor < o.m_Minor)
			{
				return -1;
			}
			else if (m_Revision > o.m_Revision)
			{
				return 1;
			}
			else if (m_Revision < o.m_Revision)
			{
				return -1;
			}
			else if (m_Patch > o.m_Patch)
			{
				return 1;
			}
			else if (m_Patch < o.m_Patch)
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

			ClientVersion a = x as ClientVersion;
			ClientVersion b = y as ClientVersion;

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