namespace Server.Items
{
    [Flipable(0x9AA, 0xE7D)]
    public class HeartwoodChest : LockableContainer
    {
        public override int LabelNumber => 1075503;  // Heartwood Chest

        [Constructable]
        public HeartwoodChest()
            : base(0x9AA)
        {
            Hue = 1193;
            Weight = 4;
            GumpID = 0x43;
        }

        public HeartwoodChest(Serial serial)
            : base(serial)
        {
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            base.AddNameProperty(list);

            list.Add(1071432); // Heartwood
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
