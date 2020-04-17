namespace Server.Items
{
    public class RaptorTeeth : Item, ICommodity
    {
        [Constructable]
        public RaptorTeeth()
            : this(1)
        {
        }

        [Constructable]
        public RaptorTeeth(int amount)
            : base(0x5747)
        {
            Stackable = true;
            Amount = amount;
        }

        public RaptorTeeth(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1113360;// raptor teeth
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
