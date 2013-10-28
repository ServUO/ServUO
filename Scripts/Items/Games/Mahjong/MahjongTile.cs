using System;

namespace Server.Engines.Mahjong
{
    public class MahjongTile
    {
        protected Point2D m_Position;
        private readonly MahjongGame m_Game;
        private readonly int m_Number;
        private readonly MahjongTileType m_Value;
        private int m_StackLevel;
        private MahjongPieceDirection m_Direction;
        private bool m_Flipped;
        public MahjongTile(MahjongGame game, int number, MahjongTileType value, Point2D position, int stackLevel, MahjongPieceDirection direction, bool flipped)
        {
            this.m_Game = game;
            this.m_Number = number;
            this.m_Value = value;
            this.m_Position = position;
            this.m_StackLevel = stackLevel;
            this.m_Direction = direction;
            this.m_Flipped = flipped;
        }

        public MahjongTile(MahjongGame game, GenericReader reader)
        {
            this.m_Game = game;

            int version = reader.ReadInt();

            this.m_Number = reader.ReadInt();
            this.m_Value = (MahjongTileType)reader.ReadInt();
            this.m_Position = reader.ReadPoint2D();
            this.m_StackLevel = reader.ReadInt();
            this.m_Direction = (MahjongPieceDirection)reader.ReadInt();
            this.m_Flipped = reader.ReadBool();
        }

        public MahjongGame Game
        {
            get
            {
                return this.m_Game;
            }
        }
        public int Number
        {
            get
            {
                return this.m_Number;
            }
        }
        public MahjongTileType Value
        {
            get
            {
                return this.m_Value;
            }
        }
        public Point2D Position
        {
            get
            {
                return this.m_Position;
            }
        }
        public int StackLevel
        {
            get
            {
                return this.m_StackLevel;
            }
        }
        public MahjongPieceDirection Direction
        {
            get
            {
                return this.m_Direction;
            }
        }
        public bool Flipped
        {
            get
            {
                return this.m_Flipped;
            }
        }
        public MahjongPieceDim Dimensions
        {
            get
            {
                return GetDimensions(this.m_Position, this.m_Direction);
            }
        }
        public bool IsMovable
        {
            get
            {
                return this.m_Game.GetStackLevel(this.Dimensions) <= this.m_StackLevel;
            }
        }
        public static MahjongPieceDim GetDimensions(Point2D position, MahjongPieceDirection direction)
        {
            if (direction == MahjongPieceDirection.Up || direction == MahjongPieceDirection.Down)
                return new MahjongPieceDim(position, 20, 30);
            else
                return new MahjongPieceDim(position, 30, 20);
        }

        public void Move(Point2D position, MahjongPieceDirection direction, bool flip, int validHandArea)
        {
            MahjongPieceDim dim = GetDimensions(position, direction);
            int curHandArea = this.Dimensions.GetHandArea();
            int newHandArea = dim.GetHandArea();

            if (!this.IsMovable || !dim.IsValid() || (validHandArea >= 0 && ((curHandArea >= 0 && curHandArea != validHandArea) || (newHandArea >= 0 && newHandArea != validHandArea))))
                return;

            this.m_Position = position;
            this.m_Direction = direction;
            this.m_StackLevel = -1; // Avoid self interference
            this.m_StackLevel = this.m_Game.GetStackLevel(dim) + 1;
            this.m_Flipped = flip;

            this.m_Game.Players.SendTilePacket(this, true, true);
        }

        public void Save(GenericWriter writer)
        {
            writer.Write((int)0); // version

            writer.Write(this.m_Number);
            writer.Write((int)this.m_Value);
            writer.Write(this.m_Position);
            writer.Write(this.m_StackLevel);
            writer.Write((int)this.m_Direction);
            writer.Write(this.m_Flipped);
        }
    }
}