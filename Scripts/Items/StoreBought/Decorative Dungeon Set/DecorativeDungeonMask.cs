namespace Server.Items
{
    [Flipable(0xA4EF, 0xA4F0)]
    public class DecorativeDungeonMask : Item
    {
        public override int LabelNumber => 1159473;  // decorative dungeon mask

        [Constructable]
        public DecorativeDungeonMask()
            : base(0xA4EF)
        {
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public DecorativeDungeonMask(Serial serial)
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
