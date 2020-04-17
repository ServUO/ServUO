namespace Server.Engines.Mahjong
{
    public class MahjongDices
    {
        private readonly MahjongGame m_Game;
        private int m_First;
        private int m_Second;
        public MahjongDices(MahjongGame game)
        {
            m_Game = game;
            m_First = Utility.Random(1, 6);
            m_Second = Utility.Random(1, 6);
        }

        public MahjongDices(MahjongGame game, GenericReader reader)
        {
            m_Game = game;

            int version = reader.ReadInt();

            m_First = reader.ReadInt();
            m_Second = reader.ReadInt();
        }

        public MahjongGame Game => m_Game;
        public int First => m_First;
        public int Second => m_Second;
        public void RollDices(Mobile from)
        {
            m_First = Utility.Random(1, 6);
            m_Second = Utility.Random(1, 6);

            m_Game.Players.SendGeneralPacket(true, true);

            if (from != null)
                m_Game.Players.SendLocalizedMessage(1062695, string.Format("{0}\t{1}\t{2}", from.Name, m_First, m_Second)); // ~1_name~ rolls the dice and gets a ~2_number~ and a ~3_number~!
        }

        public void Save(GenericWriter writer)
        {
            writer.Write(0); // version

            writer.Write(m_First);
            writer.Write(m_Second);
        }
    }
}