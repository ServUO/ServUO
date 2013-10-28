using System;

namespace Server.Items
{
    [Flipable(0xE1C, 0xFAD)]
    public class Backgammon : BaseBoard
    {
        [Constructable]
        public Backgammon()
            : base(0xE1C)
        {
        }

        public Backgammon(Serial serial)
            : base(serial)
        {
        }

        public override void CreatePieces()
        {
            for (int i = 0; i < 5; i++)
            {
                this.CreatePiece(new PieceWhiteChecker(this), 42, (17 * i) + 6);
                this.CreatePiece(new PieceBlackChecker(this), 42, (17 * i) + 119);

                this.CreatePiece(new PieceBlackChecker(this), 142, (17 * i) + 6);
                this.CreatePiece(new PieceWhiteChecker(this), 142, (17 * i) + 119);
            }

            for (int i = 0; i < 3; i++)
            {
                this.CreatePiece(new PieceBlackChecker(this), 108, (17 * i) + 6);
                this.CreatePiece(new PieceWhiteChecker(this), 108, (17 * i) + 153);
            }

            for (int i = 0; i < 2; i++)
            {
                this.CreatePiece(new PieceWhiteChecker(this), 223, (17 * i) + 6);
                this.CreatePiece(new PieceBlackChecker(this), 223, (17 * i) + 170);
            }
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