namespace Server.Items
{
    public class CoilsFang : Item
    {
        [Constructable]
        public CoilsFang()
            : base(0x10E8)
        {
            LootType = LootType.Blessed;
            Hue = 0x487;
        }

        public CoilsFang(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074229;// Coil's Fang
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