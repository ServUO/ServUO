namespace Server.Items
{
    public class DemonSkull : Item
    {
        public override bool IsArtifact => true;
        [Constructable]
        public DemonSkull()
            : base(0x224e + Utility.Random(4))
        {
        }

        public DemonSkull(Serial serial)
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