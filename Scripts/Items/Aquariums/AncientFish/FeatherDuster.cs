namespace Server.Items
{
    public class FeatherDuster : BaseFish
    {
        [Constructable]
        public FeatherDuster()
            : base(0xA381)
        {
        }

        public FeatherDuster(Serial serial)
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
