using System;

namespace Server.Engines.Mahjong
{
    public class MahjongDealerIndicator
    {
        private readonly MahjongGame m_Game;
        private Point2D m_Position;
        private MahjongPieceDirection m_Direction;
        private MahjongWind m_Wind;
        public MahjongDealerIndicator(MahjongGame game, Point2D position, MahjongPieceDirection direction, MahjongWind wind)
        {
            this.m_Game = game;
            this.m_Position = position;
            this.m_Direction = direction;
            this.m_Wind = wind;
        }

        public MahjongDealerIndicator(MahjongGame game, GenericReader reader)
        {
            this.m_Game = game;

            int version = reader.ReadInt();

            this.m_Position = reader.ReadPoint2D();
            this.m_Direction = (MahjongPieceDirection)reader.ReadInt();
            this.m_Wind = (MahjongWind)reader.ReadInt();
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
        public MahjongPieceDirection Direction
        {
            get
            {
                return this.m_Direction;
            }
        }
        public MahjongWind Wind
        {
            get
            {
                return this.m_Wind;
            }
        }
        public MahjongPieceDim Dimensions
        {
            get
            {
                return GetDimensions(this.m_Position, this.m_Direction);
            }
        }
        public static MahjongPieceDim GetDimensions(Point2D position, MahjongPieceDirection direction)
        {
            if (direction == MahjongPieceDirection.Up || direction == MahjongPieceDirection.Down)
                return new MahjongPieceDim(position, 40, 20);
            else
                return new MahjongPieceDim(position, 20, 40);
        }

        public void Move(Point2D position, MahjongPieceDirection direction, MahjongWind wind)
        {
            MahjongPieceDim dim = GetDimensions(position, direction);

            if (!dim.IsValid())
                return;

            this.m_Position = position;
            this.m_Direction = direction;
            this.m_Wind = wind;

            this.m_Game.Players.SendGeneralPacket(true, true);
        }

        public void Save(GenericWriter writer)
        {
            writer.Write((int)0); // version

            writer.Write(this.m_Position);
            writer.Write((int)this.m_Direction);
            writer.Write((int)this.m_Wind);
        }
    }
}