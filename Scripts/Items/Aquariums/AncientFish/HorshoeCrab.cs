namespace Server.Items
{
    public class HorseshoeCrab : BaseFish
    {
        [Constructable]
        public HorseshoeCrab()
            : base(0xA380)
        {
        }

        public HorseshoeCrab(Serial serial)
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
