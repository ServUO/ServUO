namespace Server.Items
{
    public class Shrimp : BaseFish
    {
        [Constructable]
        public Shrimp()
            : base(0x3B14)
        {
        }

        public Shrimp(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074596;// Shrimp
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