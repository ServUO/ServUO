namespace Server.Items
{
    public class MetalKeg : Keg
    {
        public override int LabelNumber => 1150675;

        [Constructable]
        public MetalKeg()
        {
        }

        public MetalKeg(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}