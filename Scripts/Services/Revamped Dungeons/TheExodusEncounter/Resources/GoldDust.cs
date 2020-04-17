namespace Server.Items
{
    public class GoldDust : Item, ICommodity
    {
        [Constructable]
        public GoldDust() : this(1)
        {
        }

        [Constructable]
        public GoldDust(int amount) : base(0x4C09)
        {
            Stackable = true;
            Hue = 1177;
            Weight = 1.0;
            Amount = amount;
        }

        public GoldDust(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1153504;  // gold dust

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
