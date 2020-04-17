namespace Server.Items
{
    public class Chessboard : BaseBoard
    {
        [Constructable]
        public Chessboard()
            : base(0xFA6)
        {
        }

        public Chessboard(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1016450;// a chessboard
        public override void CreatePieces()
        {
            for (int i = 0; i < 8; i++)
            {
                CreatePiece(new PieceBlackPawn(this), 67, (25 * i) + 17);
                CreatePiece(new PieceWhitePawn(this), 192, (25 * i) + 17);
            }

            // Rook
            CreatePiece(new PieceBlackRook(this), 42, 5);
            CreatePiece(new PieceBlackRook(this), 42, 180);

            CreatePiece(new PieceWhiteRook(this), 216, 5);
            CreatePiece(new PieceWhiteRook(this), 216, 180);

            // Knight
            CreatePiece(new PieceBlackKnight(this), 42, 30);
            CreatePiece(new PieceBlackKnight(this), 42, 155);

            CreatePiece(new PieceWhiteKnight(this), 216, 30);
            CreatePiece(new PieceWhiteKnight(this), 216, 155);

            // Bishop
            CreatePiece(new PieceBlackBishop(this), 42, 55);
            CreatePiece(new PieceBlackBishop(this), 42, 130);

            CreatePiece(new PieceWhiteBishop(this), 216, 55);
            CreatePiece(new PieceWhiteBishop(this), 216, 130);

            // Queen
            CreatePiece(new PieceBlackQueen(this), 42, 105);
            CreatePiece(new PieceWhiteQueen(this), 216, 105);

            // King
            CreatePiece(new PieceBlackKing(this), 42, 80);
            CreatePiece(new PieceWhiteKing(this), 216, 80);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}