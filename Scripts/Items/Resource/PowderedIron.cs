namespace Server.Items
{
    public class PowderedIron : Item
    {
        [Constructable]
        public PowderedIron()
            : this(1)
        {
        }

        [Constructable]
        public PowderedIron(int amount)
            : base(0x573D)
        {
            Stackable = true;
            Amount = amount;
        }

        public PowderedIron(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113353;// powdered iron
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