namespace Server.Items
{
    public class BandedCoralShrimp : BaseFish
    {
        [Constructable]
        public BandedCoralShrimp()
            : base(0xA37B)
        {
        }

        public BandedCoralShrimp(Serial serial)
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
