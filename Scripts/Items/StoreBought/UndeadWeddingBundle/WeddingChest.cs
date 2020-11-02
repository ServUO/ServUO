namespace Server.Items
{
    [Furniture]
    [Flipable(0x9F94, 0x9F95)]
    public class WeddingChest : LockableContainer
    {
        public override int LabelNumber => 1124876; // Wedding Chest

        [Constructable]
        public WeddingChest()
            : base(0x9F94)
        {
            GumpID = 0x266A;
            Weight = 3.0;

            DropItem(new UndeadWeddingHat());
            DropItem(new UndeadWeddingVeil());
        }

        public WeddingChest(Serial serial)
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
