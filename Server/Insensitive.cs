#region References
using System.Collections;
#endregion

namespace Server
{
	public static class Insensitive
	{
		public static IComparer Comparer { get; } = CaseInsensitiveComparer.DefaultInvariant;

		public static int Compare(string a, string b)
		{
			return Comparer.Compare(a, b);
		}

		public static bool Equals(string a, string b)
		{
			if (a == null && b == null)
			{
				return true;
			}

			if (a == null || b == null || a.Length != b.Length)
			{
				return false;
			}

			return Comparer.Compare(a, b) == 0;
		}

		public static bool StartsWith(string a, string b)
		{
			if (a == null || b == null || a.Length < b.Length)
			{
				return false;
			}

			return Comparer.Compare(a.Substring(0, b.Length), b) == 0;
		}

		public static bool EndsWith(string a, string b)
		{
			if (a == null || b == null || a.Length < b.Length)
			{
				return false;
			}

			return Comparer.Compare(a.Substring(a.Length - b.Length), b) == 0;
		}

		public static bool Contains(string a, string b)
		{
			if (a == null || b == null || a.Length < b.Length)
			{
				return false;
			}

			a = a.ToLower();
			b = b.ToLower();

			return a.IndexOf(b) >= 0;
		}
	}
}