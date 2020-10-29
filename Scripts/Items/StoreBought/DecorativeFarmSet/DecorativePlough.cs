namespace Server.Items
{
    [Flipable(0xA31F, 0xA320)]
    public class DecorativePlough : Item
    {
        public override int LabelNumber => 1125783; // plow

        [Constructable]
        public DecorativePlough()
            : base(0xA31F)
        {
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public DecorativePlough(Serial serial)
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
            reader.ReadInt();
        }
    }
}
