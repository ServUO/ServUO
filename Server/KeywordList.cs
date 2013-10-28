#region Header
// **********
// ServUO - KeywordList.cs
// **********
#endregion

namespace Server
{
	public class KeywordList
	{
		private int[] m_Keywords;
		private int m_Count;

		public KeywordList()
		{
			m_Keywords = new int[8];
			m_Count = 0;
		}

		public int Count { get { return m_Count; } }

		public bool Contains(int keyword)
		{
			bool contains = false;

			for (int i = 0; !contains && i < m_Count; ++i)
			{
				contains = (keyword == m_Keywords[i]);
			}

			return contains;
		}

		public void Add(int keyword)
		{
			if ((m_Count + 1) > m_Keywords.Length)
			{
				var old = m_Keywords;
				m_Keywords = new int[old.Length * 2];

				for (int i = 0; i < old.Length; ++i)
				{
					m_Keywords[i] = old[i];
				}
			}

			m_Keywords[m_Count++] = keyword;
		}

		private static readonly int[] m_EmptyInts = new int[0];

		public int[] ToArray()
		{
			if (m_Count == 0)
			{
				return m_EmptyInts;
			}

			var keywords = new int[m_Count];

			for (int i = 0; i < m_Count; ++i)
			{
				keywords[i] = m_Keywords[i];
			}

			m_Count = 0;

			return keywords;
		}
	}
}