namespace Server.Items
{
    public class SergeantMajor : BaseFish
    {
        [Constructable]
        public SergeantMajor()
            : base(0xA360)
        {
        }

        public SergeantMajor(Serial serial)
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
