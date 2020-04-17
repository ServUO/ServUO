namespace Server.Items
{
    [Furniture]
    [Flipable(0x0A2C, 0x0A34)]
    public class ChestOfDrawers : LockableContainer
    {
        [Constructable]
        public ChestOfDrawers()
            : base(0x0A2C)
        {
            Weight = 25.0;
        }

        public ChestOfDrawers(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultGumpID => 0x51;
        public override int DefaultDropSound => 0x42;
        public override int LabelNumber => 1022604;// chest of drawers

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