namespace Server.Items
{
    [Flipable(0xA4E8, 0xA4E9)]
    public class DecorativeWallHook : Item
    {
        public override int LabelNumber => 1159469;  // decorative wall hook

        [Constructable]
        public DecorativeWallHook()
            : base(0xA4E8)
        {
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public DecorativeWallHook(Serial serial)
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
            int version = reader.ReadInt();
        }
    }
}
