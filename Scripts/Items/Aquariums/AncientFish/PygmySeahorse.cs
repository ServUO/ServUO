namespace Server.Items
{
    public class PygmySeahorse : BaseFish
    {
        [Constructable]
        public PygmySeahorse()
            : base(0xA372)
        {
        }

        public PygmySeahorse(Serial serial)
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
