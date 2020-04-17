namespace Server.Items
{
    public class Lodestone : Item, ICommodity
    {
        [Constructable]
        public Lodestone()
            : this(1)
        {
        }

        [Constructable]
        public Lodestone(int amount)
            : base(0x5739)
        {
            Stackable = true;
            Amount = amount;
        }

        public Lodestone(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1113348;// lodestone
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
