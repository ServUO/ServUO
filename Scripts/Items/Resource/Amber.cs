namespace Server.Items
{
    public class Amber : Item, ICommodity
    {
        [Constructable]
        public Amber()
            : this(1)
        {
        }

        [Constructable]
        public Amber(int amount)
            : base(0xF25)
        {
            Stackable = true;
            Amount = amount;
        }

        public Amber(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override double DefaultWeight => 0.1;
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
