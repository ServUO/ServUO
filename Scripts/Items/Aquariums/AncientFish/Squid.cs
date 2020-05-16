namespace Server.Items
{
    public class Squid : BaseFish
    {
        [Constructable]
        public Squid()
            : base(0xA383)
        {
        }

        public Squid(Serial serial)
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
