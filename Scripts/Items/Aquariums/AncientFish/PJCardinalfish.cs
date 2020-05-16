namespace Server.Items
{
    public class PJCardinalfish : BaseFish
    {
        [Constructable]
        public PJCardinalfish()
            : base(0xA366)
        {
        }

        public PJCardinalfish(Serial serial)
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
