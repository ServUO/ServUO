namespace Server.Items
{
    public class WoodPulp : Item
    {
        [Constructable]
        public WoodPulp()
            : this(1)
        {
        }

        [Constructable]
        public WoodPulp(int amount)
            : base(0x103D)
        {
            Stackable = true;
            Amount = amount;
        }

        public WoodPulp(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113136;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}