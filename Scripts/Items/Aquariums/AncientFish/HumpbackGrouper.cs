namespace Server.Items
{
    public class HumpbackGrouper : BaseFish
    {
        [Constructable]
        public HumpbackGrouper()
            : base(0xA363)
        {
        }

        public HumpbackGrouper(Serial serial)
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
