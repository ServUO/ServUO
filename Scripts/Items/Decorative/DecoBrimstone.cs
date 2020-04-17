namespace Server.Items
{
    public class DecoBrimstone : Item
    {
        [Constructable]
        public DecoBrimstone()
            : base(0xF7F)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoBrimstone(Serial serial)
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