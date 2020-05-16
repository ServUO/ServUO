namespace Server.Items
{
    public class OrangeElephantEarSponge : BaseFish
    {
        [Constructable]
        public OrangeElephantEarSponge()
            : base(0xA389)
        {
        }

        public OrangeElephantEarSponge(Serial serial)
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
