using System;
using System.Collections;

namespace Server.Engines.Mahjong
{
    public class MahjongTileTypeGenerator
    {
        private readonly ArrayList m_LeftTileTypes;
        public MahjongTileTypeGenerator(int count)
        {
            this.m_LeftTileTypes = new ArrayList(34 * count);

            for (int i = 1; i <= 34; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    this.m_LeftTileTypes.Add((MahjongTileType)i);
                }
            }
        }

        public ArrayList LeftTileTypes
        {
            get
            {
                return this.m_LeftTileTypes;
            }
        }
        public MahjongTileType Next()
        {
            int random = Utility.Random(this.m_LeftTileTypes.Count);
            MahjongTileType next = (MahjongTileType)this.m_LeftTileTypes[random];
            this.m_LeftTileTypes.RemoveAt(random);

            return next;
        }
    }
}