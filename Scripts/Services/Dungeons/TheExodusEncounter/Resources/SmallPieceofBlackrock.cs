namespace Server.Items
{
    public class SmallPieceofBlackrock : Item, ICommodity
    {
        [Constructable]
        public SmallPieceofBlackrock() : this(1)
        {
        }

        [Constructable]
        public SmallPieceofBlackrock(int amount) : base(0x0F28)
        {
            Hue = 1175;
            Stackable = true;
            Amount = amount;
            Weight = 1.0;
        }

        public SmallPieceofBlackrock(Serial serial) : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1150016;

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
