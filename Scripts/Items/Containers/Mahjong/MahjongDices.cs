using System;

namespace Server.Engines.Mahjong
{
    public class MahjongDices
    {
        private readonly MahjongGame m_Game;
        private int m_First;
        private int m_Second;
        public MahjongDices(MahjongGame game)
        {
            this.m_Game = game;
            this.m_First = Utility.Random(1, 6);
            this.m_Second = Utility.Random(1, 6);
        }

        public MahjongDices(MahjongGame game, GenericReader reader)
        {
            this.m_Game = game;

            int version = reader.ReadInt();

            this.m_First = reader.ReadInt();
            this.m_Second = reader.ReadInt();
        }

        public MahjongGame Game
        {
            get
            {
                return this.m_Game;
            }
        }
        public int First
        {
            get
            {
                return this.m_First;
            }
        }
        public int Second
        {
            get
            {
                return this.m_Second;
            }
        }
        public void RollDices(Mobile from)
        {
            this.m_First = Utility.Random(1, 6);
            this.m_Second = Utility.Random(1, 6);

            this.m_Game.Players.SendGeneralPacket(true, true);

            if (from != null)
                this.m_Game.Players.SendLocalizedMessage(1062695, string.Format("{0}\t{1}\t{2}", from.Name, this.m_First, this.m_Second)); // ~1_name~ rolls the dice and gets a ~2_number~ and a ~3_number~!
        }

        public void Save(GenericWriter writer)
        {
            writer.Write((int)0); // version

            writer.Write(this.m_First);
            writer.Write(this.m_Second);
        }
    }
}