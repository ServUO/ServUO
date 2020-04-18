namespace Server.Items
{
    public class ArtifactLargeVase : Item
    {
        public override bool IsArtifact => true;
        [Constructable]
        public ArtifactLargeVase()
            : base(0x0B47)
        {
        }

        public ArtifactLargeVase(Serial serial)
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