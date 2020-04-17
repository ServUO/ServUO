namespace Server.Items
{
    public class DecoSpittoon : Item
    {
        [Constructable]
        public DecoSpittoon()
            : base(0x1003)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoSpittoon(Serial serial)
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