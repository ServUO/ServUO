namespace Server.Items
{
    public class HippoTang : BaseFish
    {
        [Constructable]
        public HippoTang()
            : base(0xA391)
        {
        }

        public HippoTang(Serial serial)
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
