namespace Server.Items
{
    public class FeyWings : Item, ICommodity
    {
        public override int LabelNumber => 1113332;  // fey wings
        public override double DefaultWeight => 0.1;

        [Constructable]
        public FeyWings()
            : this(1)
        {
        }

        [Constructable]
        public FeyWings(int amount)
            : base(0x5726)
        {
            Stackable = true;
            Amount = amount;
        }

        public FeyWings(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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
