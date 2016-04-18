using System;

namespace Server.Engines.Mahjong
{
    public class MahjongWallBreakIndicator
    {
        private readonly MahjongGame m_Game;
        private Point2D m_Position;
        public MahjongWallBreakIndicator(MahjongGame game, Point2D position)
        {
            this.m_Game = game;
            this.m_Position = position;
        }

        public MahjongWallBreakIndicator(MahjongGame game, GenericReader reader)
        {
            this.m_Game = game;

            int version = reader.ReadInt();

            this.m_Position = reader.ReadPoint2D();
        }

        public MahjongGame Game
        {
            get
            {
                return this.m_Game;
            }
        }
        public Point2D Position
        {
            get
            {
                return this.m_Position;
            }
        }
        public MahjongPieceDim Dimensions
        {
            get
            {
                return GetDimensions(this.m_Position);
            }
        }
        public static MahjongPieceDim GetDimensions(Point2D position)
        {
            return new MahjongPieceDim(position, 20, 20);
        }

        public void Move(Point2D position)
        {
            MahjongPieceDim dim = GetDimensions(position);

            if (!dim.IsValid())
                return;

            this.m_Position = position;

            this.m_Game.Players.SendGeneralPacket(true, true);
        }

        public void Save(GenericWriter writer)
        {
            writer.Write((int)0); // version

            writer.Write(this.m_Position);
        }
    }
}