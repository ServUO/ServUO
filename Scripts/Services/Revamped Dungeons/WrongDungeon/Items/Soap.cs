namespace Server.Items
{
    public class Soap : Item
    {
        public override int LabelNumber => 1152267;  // soap

        [Constructable]
        public Soap()
            : this(1)
        {
        }

        [Constructable]
        public Soap(int amount)
            : base(0x1422)
        {
            Hue = 1285;
            Weight = 1.0;
            Stackable = true;
            Amount = amount;
        }

        public Soap(Serial serial)
            : base(serial)
        {
        }

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