namespace Server
{
	public class KeywordList
	{
		private int[] m_Keywords;

		public int Count { get; private set; }

		public KeywordList()
		{
			m_Keywords = new int[8];
		}

		public bool Contains(int keyword)
		{
			var contains = false;

			for (var i = 0; !contains && i < Count; ++i)
			{
				contains = keyword == m_Keywords[i];
			}

			return contains;
		}

		public void Add(int keyword)
		{
			if (Count + 1 > m_Keywords.Length)
			{
				var old = m_Keywords;
				m_Keywords = new int[old.Length * 2];

				for (var i = 0; i < old.Length; ++i)
				{
					m_Keywords[i] = old[i];
				}
			}

			m_Keywords[Count++] = keyword;
		}

		private static readonly int[] m_EmptyInts = new int[0];

		public int[] ToArray()
		{
			if (Count == 0)
			{
				return m_EmptyInts;
			}

			var keywords = new int[Count];

			for (var i = 0; i < Count; ++i)
			{
				keywords[i] = m_Keywords[i];
			}

			Count = 0;

			return keywords;
		}
	}
}