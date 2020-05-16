namespace Server.Items
{
    public class SpottedParrotfish : BaseFish
    {
        [Constructable]
        public SpottedParrotfish()
            : base(0xA364)
        {
        }

        public SpottedParrotfish(Serial serial)
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
