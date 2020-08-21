namespace Server.Items
{
    [Furniture]
    [Flipable(0xA588, 0xA58E)]
    public class DecorativeMagicBookStand : Item
    {
        public override int LabelNumber => 1072874;  // Book Stand

        [Constructable]
        public DecorativeMagicBookStand()
            : base(0xA588)
        {
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public DecorativeMagicBookStand(Serial serial)
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
