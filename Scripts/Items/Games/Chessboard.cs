using System;

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

        public override int LabelNumber
        {
            get
            {
                return 1016450;
            }
        }// a chessboard
        public override void CreatePieces()
        {
            for (int i = 0; i < 8; i++)
            {
                this.CreatePiece(new PieceBlackPawn(this), 67, (25 * i) + 17);
                this.CreatePiece(new PieceWhitePawn(this), 192, (25 * i) + 17);
            }

            // Rook
            this.CreatePiece(new PieceBlackRook(this), 42, 5);
            this.CreatePiece(new PieceBlackRook(this), 42, 180);

            this.CreatePiece(new PieceWhiteRook(this), 216, 5);
            this.CreatePiece(new PieceWhiteRook(this), 216, 180);

            // Knight
            this.CreatePiece(new PieceBlackKnight(this), 42, 30);
            this.CreatePiece(new PieceBlackKnight(this), 42, 155);

            this.CreatePiece(new PieceWhiteKnight(this), 216, 30);
            this.CreatePiece(new PieceWhiteKnight(this), 216, 155);
					
            // Bishop
            this.CreatePiece(new PieceBlackBishop(this), 42, 55);
            this.CreatePiece(new PieceBlackBishop(this), 42, 130);

            this.CreatePiece(new PieceWhiteBishop(this), 216, 55);
            this.CreatePiece(new PieceWhiteBishop(this), 216, 130);
			
            // Queen
            this.CreatePiece(new PieceBlackQueen(this), 42, 105);
            this.CreatePiece(new PieceWhiteQueen(this), 216, 105);

            // King
            this.CreatePiece(new PieceBlackKing(this), 42, 80);
            this.CreatePiece(new PieceWhiteKing(this), 216, 80);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}