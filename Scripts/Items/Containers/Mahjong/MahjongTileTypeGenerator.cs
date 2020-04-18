using System.Collections;

namespace Server.Engines.Mahjong
{
    public class MahjongTileTypeGenerator
    {
        private readonly ArrayList m_LeftTileTypes;
        public MahjongTileTypeGenerator(int count)
        {
            m_LeftTileTypes = new ArrayList(34 * count);

            for (int i = 1; i <= 34; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    m_LeftTileTypes.Add((MahjongTileType)i);
                }
            }
        }

        public ArrayList LeftTileTypes => m_LeftTileTypes;
        public MahjongTileType Next()
        {
            int random = Utility.Random(m_LeftTileTypes.Count);
            MahjongTileType next = (MahjongTileType)m_LeftTileTypes[random];
            m_LeftTileTypes.RemoveAt(random);

            return next;
        }
    }
}