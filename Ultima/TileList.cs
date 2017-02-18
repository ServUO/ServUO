#region References
using System.Collections.Generic;
#endregion

namespace Ultima
{
	public sealed class HuedTileList
	{
		private readonly List<HuedTile> m_Tiles;

		public HuedTileList()
		{
			m_Tiles = new List<HuedTile>();
		}

		public int Count { get { return m_Tiles.Count; } }

		public void Add(ushort id, short hue, sbyte z)
		{
			m_Tiles.Add(new HuedTile(id, hue, z));
		}

		public HuedTile[] ToArray()
		{
			var tiles = new HuedTile[Count];

			if (m_Tiles.Count > 0)
			{
				m_Tiles.CopyTo(tiles);
			}
			m_Tiles.Clear();

			return tiles;
		}
	}

	public sealed class TileList
	{
		private readonly List<Tile> m_Tiles;
	    private readonly Art art ;
		public TileList(Art art)
		{
		    this.art = art;
			m_Tiles = new List<Tile>();
		}

		public int Count { get { return m_Tiles.Count; } }

		public void Add(ushort id, sbyte z)
		{
			m_Tiles.Add(new Tile(id, z));
		}

		public void Add(ushort id, sbyte z, sbyte flag)
		{
			m_Tiles.Add(new Tile(id, z, flag));
		}

		public Tile[] ToArray()
		{
			var tiles = new Tile[Count];
			if (m_Tiles.Count > 0)
			{
				m_Tiles.CopyTo(tiles);
			}
			m_Tiles.Clear();

			return tiles;
		}

		public Tile Get(int i)
		{
			return m_Tiles[i];
		}
	}

	public sealed class MTileList
	{
		private readonly List<MTile> m_Tiles;
		public MTileList()
		{
			m_Tiles = new List<MTile>();
		}

		public int Count => m_Tiles.Count;

	    public void Add(ushort id, sbyte z, ItemData ourItemData, ItemData theirItemData, ushort artGetLegalItemId)
		{
			m_Tiles.Add(new MTile(id, z, ourItemData, theirItemData, artGetLegalItemId));
		}

		public void Add(ushort id, sbyte z, sbyte flag, ItemData ourItemData, ItemData theirItemData, ushort artGetLegalItemId)
		{
			m_Tiles.Add(new MTile(id, z, flag, ourItemData, theirItemData, artGetLegalItemId));
		}

		public void Add(ushort id, sbyte z, sbyte flag, int unk1, ItemData ourItemData, ItemData theirItemData, ushort artGetLegalItemId)
		{
			m_Tiles.Add(new MTile(id, z, flag, unk1, ourItemData, theirItemData, artGetLegalItemId));
		}

	    public void Add(MTile mtle)
	    {
	        m_Tiles.Add(mtle);
	    }

		public MTile[] ToArray()
		{
			var tiles = new MTile[Count];

			if (m_Tiles.Count > 0)
			{
				m_Tiles.CopyTo(tiles);
			}
			m_Tiles.Clear();

			return tiles;
		}

		public MTile Get(int i)
		{
			return m_Tiles[i];
		}

		public void Set(int i, ushort id, sbyte z, ushort artGetLegalItemId)
		{
			if (i < Count)
			{
				m_Tiles[i].Set(id, z, artGetLegalItemId);
			}
		}

		public void Set(int i, ushort id, sbyte z, sbyte flag, ushort artGetLegalItemId)
		{
			if (i < Count)
			{
				m_Tiles[i].Set(id, z, flag, artGetLegalItemId);
			}
		}

		public void Set(int i, ushort id, sbyte z, sbyte flag, int unk1, ushort artGetLegalItemId)
		{
			if (i < Count)
			{
				m_Tiles[i].Set(id, z, flag, unk1, artGetLegalItemId);
			}
		}

		public void Remove(int i)
		{
			if (i < Count)
			{
				m_Tiles.RemoveAt(i);
			}
		}
	}
}