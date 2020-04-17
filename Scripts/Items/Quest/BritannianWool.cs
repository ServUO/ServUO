namespace Server.Items
{
    public class BritannianWool : Wool
    {
        public override int LabelNumber => 1113242;  // Britannian wool

        [Constructable]
        public BritannianWool() : this(1)
        {
        }

        [Constructable]
        public BritannianWool(int amount) : base(0x101F)
        {
            Stackable = true;
            Hue = 992;
            Weight = 4.0;
            Amount = amount;
        }

        public BritannianWool(Serial serial) : base(serial)
        {
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