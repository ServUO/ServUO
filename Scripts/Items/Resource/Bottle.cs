namespace Server.Items
{
    public class Bottle : Item, ICommodity
    {
        [Constructable]
        public Bottle()
            : this(1)
        {
        }

        [Constructable]
        public Bottle(int amount)
            : base(0xF0E)
        {
            Stackable = true;
            Weight = 1.0;
            Amount = amount;
        }

        public Bottle(Serial serial)
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
