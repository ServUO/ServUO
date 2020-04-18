namespace Server.Items
{
    public class ArtifactVase : Item
    {
        public override bool IsArtifact => true;
        [Constructable]
        public ArtifactVase()
            : base(0x0B48)
        {
        }

        public ArtifactVase(Serial serial)
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