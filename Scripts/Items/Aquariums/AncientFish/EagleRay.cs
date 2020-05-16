namespace Server.Items
{
    public class EagleRay : BaseFish
    {
        [Constructable]
        public EagleRay()
            : base(0xA373)
        {
        }

        public EagleRay(Serial serial)
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
