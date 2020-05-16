namespace Server.Items
{
    public class MantisShrimp : BaseFish
    {
        [Constructable]
        public MantisShrimp()
            : base(0xA37D)
        {
        }

        public MantisShrimp(Serial serial)
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
