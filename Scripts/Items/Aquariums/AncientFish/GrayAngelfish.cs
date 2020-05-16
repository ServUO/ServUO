namespace Server.Items
{
    public class GrayAngelfish : BaseFish
    {
        [Constructable]
        public GrayAngelfish()
            : base(0xA375)
        {
        }

        public GrayAngelfish(Serial serial)
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
