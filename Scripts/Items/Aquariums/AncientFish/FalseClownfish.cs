namespace Server.Items
{
    public class FalseClownfish : BaseFish
    {
        [Constructable]
        public FalseClownfish()
            : base(0xA362)
        {
        }

        public FalseClownfish(Serial serial)
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
