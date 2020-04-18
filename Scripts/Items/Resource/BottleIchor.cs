namespace Server.Items
{
    public class BottleIchor : Item, ICommodity
    {
        [Constructable]
        public BottleIchor()
            : this(1)
        {
        }

        [Constructable]
        public BottleIchor(int amount)
            : base(0x5748)
        {
            Stackable = true;
            Amount = amount;
        }

        public BottleIchor(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1113361;// bottle of ichor
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
