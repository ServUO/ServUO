namespace Server.Items
{
    public class PortraitOfTheBride : Item
    {
        public override int LabelNumber => 1075300; // Portrait of the Bride

        [Constructable]
        public PortraitOfTheBride()
            : base(0xE9F)
        {
            LootType = LootType.Blessed;
            Weight = 10.0;
        }

        public PortraitOfTheBride(Serial serial)
            : base(serial)
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