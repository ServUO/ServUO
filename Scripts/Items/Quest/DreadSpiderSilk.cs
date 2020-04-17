namespace Server.Items
{
    public class DreadSpiderSilk : Item
    {
        [Constructable]
        public DreadSpiderSilk()
            : base(0xDF8)
        {
            LootType = LootType.Blessed;
            Weight = 4.0;
            Hue = 0x481;
        }

        public DreadSpiderSilk(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075319;// Dread Spider Silk
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