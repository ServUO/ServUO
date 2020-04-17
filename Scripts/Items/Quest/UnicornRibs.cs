namespace Server.Items
{
    public class UnicornRibs : Item
    {
        [Constructable]
        public UnicornRibs()
            : base(0x9F1)
        {
            LootType = LootType.Blessed;
            Weight = 1;
            Hue = 0x14B;
        }

        public UnicornRibs(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074611;// Unicorn Ribs
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