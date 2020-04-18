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
            m_Game = game;
            m_Number = number;
            m_Value = value;
            m_Position = position;
            m_StackLevel = stackLevel;
            m_Direction = direction;
            m_Flipped = flipped;
        }

        public MahjongTile(MahjongGame game, GenericReader reader)
        {
            m_Game = game;

            int version = reader.ReadInt();

            m_Number = reader.ReadInt();
            m_Value = (MahjongTileType)reader.ReadInt();
            m_Position = reader.ReadPoint2D();
            m_StackLevel = reader.ReadInt();
            m_Direction = (MahjongPieceDirection)reader.ReadInt();
            m_Flipped = reader.ReadBool();
        }

        public MahjongGame Game => m_Game;
        public int Number => m_Number;
        public MahjongTileType Value => m_Value;
        public Point2D Position => m_Position;
        public int StackLevel => m_StackLevel;
        public MahjongPieceDirection Direction => m_Direction;
        public bool Flipped => m_Flipped;
        public MahjongPieceDim Dimensions => GetDimensions(m_Position, m_Direction);
        public bool IsMovable => m_Game.GetStackLevel(Dimensions) <= m_StackLevel;
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
            int curHandArea = Dimensions.GetHandArea();
            int newHandArea = dim.GetHandArea();

            if (!IsMovable || !dim.IsValid() || (validHandArea >= 0 && ((curHandArea >= 0 && curHandArea != validHandArea) || (newHandArea >= 0 && newHandArea != validHandArea))))
                return;

            m_Position = position;
            m_Direction = direction;
            m_StackLevel = -1; // Avoid self interference
            m_StackLevel = m_Game.GetStackLevel(dim) + 1;
            m_Flipped = flip;

            m_Game.Players.SendTilePacket(this, true, true);
        }

        public void Save(GenericWriter writer)
        {
            writer.Write(0); // version

            writer.Write(m_Number);
            writer.Write((int)m_Value);
            writer.Write(m_Position);
            writer.Write(m_StackLevel);
            writer.Write((int)m_Direction);
            writer.Write(m_Flipped);
        }
    }
}