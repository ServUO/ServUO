namespace Server.Items
{
    public class KingsPainting1 : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override bool ShowArtifactRarity => false;
        public override int ArtifactRarity => 8;

        public override double DefaultWeight => 10.0;
        public override string DefaultName => "A Painting From The Personal Collection Of The King";

        [Constructable]
        public KingsPainting1()
            : base(19552)
        {
        }

        public KingsPainting1(Serial serial)
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

    public class KingsPainting2 : BaseDecorationArtifact
    {
        public override bool IsArtifact => true;
        public override bool ShowArtifactRarity => false;
        public override int ArtifactRarity => 8;

        public override double DefaultWeight => 10.0;
        public override string DefaultName => "A Painting From The Personal Collection Of The King";

        [Constructable]
        public KingsPainting2()
            : base(19558)
        {
        }

        public KingsPainting2(Serial serial)
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
