namespace Server.Items
{
    public class ElkhornCoral : BaseFish
    {
        [Constructable]
        public ElkhornCoral()
            : base(0xA38E)
        {
        }

        public ElkhornCoral(Serial serial)
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
