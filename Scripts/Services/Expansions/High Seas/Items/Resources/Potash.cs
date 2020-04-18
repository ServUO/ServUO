namespace Server.Items
{
    public class Potash : Item, ICommodity
    {
        public override int LabelNumber => 1116319;  // potash

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        [Constructable]
        public Potash()
            : this(1)
        {
        }

        [Constructable]
        public Potash(int amount)
            : base(0x423A)
        {
            Stackable = true;
            Amount = amount;
            Hue = 1102;
        }

        public Potash(Serial serial)
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
