namespace Server.Items
{
    public class SeaUrchin : BaseFish
    {
        [Constructable]
        public SeaUrchin()
            : base(0xA388)
        {
        }

        public SeaUrchin(Serial serial)
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
