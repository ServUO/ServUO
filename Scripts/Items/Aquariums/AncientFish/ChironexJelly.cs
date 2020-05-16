namespace Server.Items
{
    public class ChironexJelly : BaseFish
    {
        [Constructable]
        public ChironexJelly()
            : base(0xA392)
        {
        }

        public ChironexJelly(Serial serial)
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

            int version = reader.ReadInt();
        }
    }
}
