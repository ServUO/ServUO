namespace Server.Items
{
    public class PixieLeg : Item
    {
        [Constructable]
        public PixieLeg()
            : this(1)
        {
        }

        [Constructable]
        public PixieLeg(int amount)
            : base(0x1608)
        {
            LootType = LootType.Blessed;
            Weight = 1;
            Hue = 0x1C2;

            Stackable = true;
            Amount = amount;
        }

        public PixieLeg(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074613;// Pixie Leg
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