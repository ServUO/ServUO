namespace Server.Items
{
    [Furniture]
    [Flipable(0xA586, 0xA587)]
    public class DecorativeMageThrone : Item
    {
        public override int LabelNumber => 1098456;  // chair

        [Constructable]
        public DecorativeMageThrone()
            : base(0xA586)
        {
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public DecorativeMageThrone(Serial serial)
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
