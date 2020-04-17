namespace Server.Items
{
    [Furniture]
    [Flipable(0x0A30, 0x0A38)]
    public class FootedChestOfDrawers : LockableContainer
    {
        [Constructable]
        public FootedChestOfDrawers()
            : base(0x0A30)
        {
            Weight = 25.0;
        }

        public FootedChestOfDrawers(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultGumpID => 0x48;
        public override int DefaultDropSound => 0x42;
        public override int LabelNumber => 1151221;// footed chest of drawers

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}