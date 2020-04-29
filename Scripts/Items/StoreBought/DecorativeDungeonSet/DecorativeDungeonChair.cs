namespace Server.Items
{
    [Flipable(0xA4EA, 0xA4EB)]
    public class DecorativeDungeonChair : Item
    {
        public override int LabelNumber => 1159470;  // decorative dungeon chair

        [Constructable]
        public DecorativeDungeonChair()
            : base(0xA4EA)
        {
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public DecorativeDungeonChair(Serial serial)
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
