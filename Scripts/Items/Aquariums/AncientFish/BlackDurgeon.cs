namespace Server.Items
{
    public class BlackDurgeon : BaseFish
    {
        [Constructable]
        public BlackDurgeon()
            : base(0xA36D)
        {
        }

        public BlackDurgeon(Serial serial)
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
