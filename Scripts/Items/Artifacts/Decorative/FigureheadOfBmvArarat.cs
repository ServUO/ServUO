namespace Server.Items
{
    public class FigureheadOfBmvArarat : BaseDecorationArtifact
    {
        public override int ArtifactRarity => 8;
        public override bool IsArtifact => true;

        [Constructable]
        public FigureheadOfBmvArarat()
            : base(0x2D0E)
        {
            Name = "Figurehead Of The Bmv Ararat";
            Weight = 10.0;
            Hue = 2968; // checked
        }

        public FigureheadOfBmvArarat(Serial serial)
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