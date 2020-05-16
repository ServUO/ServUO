namespace Server.Items
{
    public class TreeCoral : BaseFish
    {
        [Constructable]
        public TreeCoral()
            : base(0xA38F)
        {
        }

        public TreeCoral(Serial serial)
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
