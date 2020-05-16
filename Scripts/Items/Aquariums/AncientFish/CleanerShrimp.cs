namespace Server.Items
{
    public class CleanerShrimp : BaseFish
    {
        [Constructable]
        public CleanerShrimp()
            : base(0xA37C)
        {
        }

        public CleanerShrimp(Serial serial)
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
