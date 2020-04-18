namespace Server.Engines.Mahjong
{
    public struct MahjongPieceDim
    {
        private readonly Point2D m_Position;
        private readonly int m_Width;
        private readonly int m_Height;
        public MahjongPieceDim(Point2D position, int width, int height)
        {
            m_Position = position;
            m_Width = width;
            m_Height = height;
        }

        public Point2D Position => m_Position;
        public int Width => m_Width;
        public int Height => m_Height;
        public bool IsValid()
        {
            return m_Position.X >= 0 && m_Position.Y >= 0 && m_Position.X + m_Width <= 670 && m_Position.Y + m_Height <= 670;
        }

        public bool IsOverlapping(MahjongPieceDim dim)
        {
            return m_Position.X < dim.m_Position.X + dim.m_Width && m_Position.Y < dim.m_Position.Y + dim.m_Height && m_Position.X + m_Width > dim.m_Position.X && m_Position.Y + m_Height > dim.m_Position.Y;
        }

        public int GetHandArea()
        {
            if (m_Position.X + m_Width > 150 && m_Position.X < 520 && m_Position.Y < 35)
                return 0;

            if (m_Position.X + m_Width > 635 && m_Position.Y + m_Height > 150 && m_Position.Y < 520)
                return 1;

            if (m_Position.X + m_Width > 150 && m_Position.X < 520 && m_Position.Y + m_Height > 635)
                return 2;

            if (m_Position.X < 35 && m_Position.Y + m_Height > 150 && m_Position.Y < 520)
                return 3;

            return -1;
        }
    }
}