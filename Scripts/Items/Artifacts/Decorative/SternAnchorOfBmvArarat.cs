namespace Server.Items
{
    public class SternAnchorOfBmvArarat : BaseDecorationArtifact
    {
        public override int ArtifactRarity => 8;
        public override bool IsArtifact => true;

        [Constructable]
        public SternAnchorOfBmvArarat()
            : base(0x14F7)
        {
            Name = "Stern Anchor of the BMV Ararat";
            Weight = 10.0;
            Hue = 2959; // checked
        }

        public SternAnchorOfBmvArarat(Serial serial)
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