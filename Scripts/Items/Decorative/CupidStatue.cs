namespace Server.Items
{
    [Flipable(0x4F7C, 0x4F7D)]
    public class CupidStatue : Item
    {
        public override int LabelNumber => 1099220;  // cupid statue

        [Constructable]
        public CupidStatue()
            : base(0x4F7C)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public CupidStatue(Serial serial)
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
