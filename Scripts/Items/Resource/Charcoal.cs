namespace Server.Items
{
    public class Charcoal : Item, ICommodity
    {
        public override int LabelNumber => 1116303;  // charcoal

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        [Constructable]
        public Charcoal()
            : this(1)
        {
        }

        [Constructable]
        public Charcoal(int amount)
            : base(0x423A)
        {
            Stackable = true;
            Amount = amount;
            Hue = 1190;
        }

        public Charcoal(Serial serial)
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
