namespace Server.Items
{
    public class CheckerBoard : BaseBoard
    {
        [Constructable]
        public CheckerBoard()
            : base(0xFA6)
        {
        }

        public CheckerBoard(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1016449;// a checker board
        public override void CreatePieces()
        {
            for (int i = 0; i < 4; i++)
            {
                CreatePiece(new PieceWhiteChecker(this), (50 * i) + 45, 25);
                CreatePiece(new PieceWhiteChecker(this), (50 * i) + 70, 50);
                CreatePiece(new PieceWhiteChecker(this), (50 * i) + 45, 75);
                CreatePiece(new PieceBlackChecker(this), (50 * i) + 70, 150);
                CreatePiece(new PieceBlackChecker(this), (50 * i) + 45, 175);
                CreatePiece(new PieceBlackChecker(this), (50 * i) + 70, 200);
            }
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