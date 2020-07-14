namespace Server.Items
{
    public class Drums : BaseInstrument
    {
        [Constructable]
        public Drums()
            : base(0xE9C, 0x38, 0x39)
        {
            Weight = 4.0;
        }

        public Drums(Serial serial)
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