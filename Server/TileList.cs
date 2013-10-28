#region Header
// **********
// ServUO - TileList.cs
// **********
#endregion

namespace Server
{
	public class TileList
	{
		private StaticTile[] m_Tiles;
		private int m_Count;

		public TileList()
		{
			m_Tiles = new StaticTile[8];
			m_Count = 0;
		}

		public int Count { get { return m_Count; } }

		public void AddRange(StaticTile[] tiles)
		{
			if ((m_Count + tiles.Length) > m_Tiles.Length)
			{
				var old = m_Tiles;
				m_Tiles = new StaticTile[(m_Count + tiles.Length) * 2];

				for (int i = 0; i < old.Length; ++i)
				{
					m_Tiles[i] = old[i];
				}
			}

			for (int i = 0; i < tiles.Length; ++i)
			{
				m_Tiles[m_Count++] = tiles[i];
			}
		}

		public void Add(ushort id, sbyte z)
		{
			if ((m_Count + 1) > m_Tiles.Length)
			{
				var old = m_Tiles;
				m_Tiles = new StaticTile[old.Length * 2];

				for (int i = 0; i < old.Length; ++i)
				{
					m_Tiles[i] = old[i];
				}
			}

			m_Tiles[m_Count].m_ID = id;
			m_Tiles[m_Count].m_Z = z;
			++m_Count;
		}

		private static readonly StaticTile[] m_EmptyTiles = new StaticTile[0];

		public StaticTile[] ToArray()
		{
			if (m_Count == 0)
			{
				return m_EmptyTiles;
			}

			var tiles = new StaticTile[m_Count];

			for (int i = 0; i < m_Count; ++i)
			{
				tiles[i] = m_Tiles[i];
			}

			m_Count = 0;

			return tiles;
		}
	}
}