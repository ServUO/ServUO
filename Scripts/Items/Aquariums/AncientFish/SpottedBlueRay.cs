namespace Server.Items
{
    public class SpottedBlueRay : BaseFish
    {
        [Constructable]
        public SpottedBlueRay()
            : base(0xA374)
        {
        }

        public SpottedBlueRay(Serial serial)
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
