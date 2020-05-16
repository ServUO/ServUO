namespace Server.Items
{
    public class StaghornCoral : BaseFish
    {
        [Constructable]
        public StaghornCoral()
            : base(0xA38D)
        {
        }

        public StaghornCoral(Serial serial)
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
