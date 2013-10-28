using System;

namespace Server.Engines.Mahjong
{
    public struct MahjongPieceDim
    {
        private readonly Point2D m_Position;
        private readonly int m_Width;
        private readonly int m_Height;
        public MahjongPieceDim(Point2D position, int width, int height)
        {
            this.m_Position = position;
            this.m_Width = width;
            this.m_Height = height;
        }

        public Point2D Position
        {
            get
            {
                return this.m_Position;
            }
        }
        public int Width
        {
            get
            {
                return this.m_Width;
            }
        }
        public int Height
        {
            get
            {
                return this.m_Height;
            }
        }
        public bool IsValid()
        {
            return this.m_Position.X >= 0 && this.m_Position.Y >= 0 && this.m_Position.X + this.m_Width <= 670 && this.m_Position.Y + this.m_Height <= 670;
        }

        public bool IsOverlapping(MahjongPieceDim dim)
        {
            return this.m_Position.X < dim.m_Position.X + dim.m_Width && this.m_Position.Y < dim.m_Position.Y + dim.m_Height && this.m_Position.X + this.m_Width > dim.m_Position.X && this.m_Position.Y + this.m_Height > dim.m_Position.Y;
        }

        public int GetHandArea()
        {
            if (this.m_Position.X + this.m_Width > 150 && this.m_Position.X < 520 && this.m_Position.Y < 35)
                return 0;

            if (this.m_Position.X + this.m_Width > 635 && this.m_Position.Y + this.m_Height > 150 && this.m_Position.Y < 520)
                return 1;

            if (this.m_Position.X + this.m_Width > 150 && this.m_Position.X < 520 && this.m_Position.Y + this.m_Height > 635)
                return 2;

            if (this.m_Position.X < 35 && this.m_Position.Y + this.m_Height > 150 && this.m_Position.Y < 520)
                return 3;

            return -1;
        }
    }
}