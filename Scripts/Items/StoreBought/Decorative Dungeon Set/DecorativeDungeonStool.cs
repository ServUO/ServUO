namespace Server.Items
{
    public class DecorativeDungeonStool : Item
    {
        public override int LabelNumber => 1159471;  // decorative dungeon stool

        [Constructable]
        public DecorativeDungeonStool()
            : base(0xA4EC)
        {
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public DecorativeDungeonStool(Serial serial)
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
