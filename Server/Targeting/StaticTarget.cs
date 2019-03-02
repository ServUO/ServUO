namespace Server.Targeting
{
	public class StaticTarget : IPoint3D
	{
        private Map m_Map;
		private Point3D m_Location;
        private readonly int m_TrueZ;
        private readonly int m_ItemID;

		public StaticTarget(Map map, Point3D location, int itemID)
		{
            m_Map = map;
            m_Location = location;
            m_TrueZ = location.m_Z;
            m_ItemID = itemID & TileData.MaxItemValue;
			m_Location.Z += TileData.ItemTable[m_ItemID].CalcHeight;
		}

		[CommandProperty(AccessLevel.Counselor)]
		public Point3D Location { get { return m_Location; } }

		[CommandProperty(AccessLevel.Counselor)]
		public string Name { get { return TileData.ItemTable[m_ItemID].Name; } }

		[CommandProperty(AccessLevel.Counselor)]
		public TileFlag Flags { get { return TileData.ItemTable[m_ItemID].Flags; } }

		[CommandProperty(AccessLevel.Counselor)]
		public int X { get { return m_Location.X; } }

		[CommandProperty(AccessLevel.Counselor)]
		public int Y { get { return m_Location.Y; } }

		[CommandProperty(AccessLevel.Counselor)]
		public int Z { get { return m_Location.Z; } }

        [CommandProperty(AccessLevel.Counselor)]
        public int TrueZ
        {
            get
            {
                return m_TrueZ;
            }
        }

        [CommandProperty(AccessLevel.Counselor)]
		public int ItemID { get { return m_ItemID; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Hue
        {
            get
            {
                if (m_Map == null || m_Map == Map.Internal)
                {
                    return 0;
                }

                StaticTile[] tile = m_Map.Tiles.GetStaticTiles(m_Location.m_X, m_Location.m_Y, false);
                int count = tile.Length;
                for (int i = 0; i < count; i++)
                {
                    StaticTile t = tile[i];
                    if (t.m_Z == m_TrueZ && t.m_ID == m_ItemID)
                    {
                        return t.m_Hue;
                    }
                }
                return 0;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map Map
        {
            get
            {
                return m_Map;
            }
        }
    }
}
