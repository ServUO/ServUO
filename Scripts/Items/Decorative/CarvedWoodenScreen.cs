namespace Server.Items
{
    [Furniture]
    [Flipable(0x1945, 0x1946)]
    public class CarvedWoodenScreen : Item
    {
        public override int LabelNumber => 1075495;  // Carved Wooden Screen

        [Constructable]
        public CarvedWoodenScreen()
            : base(0x1945)
        {
            LootType = LootType.Blessed;
            Weight = 6.0;
        }

        public CarvedWoodenScreen(Serial serial)
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
            reader.ReadInt();
        }
    }
}
