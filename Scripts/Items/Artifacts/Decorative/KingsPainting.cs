using System;

namespace Server.Items
{
    public class KingsPainting1 : BaseDecorationArtifact
    {
        public override bool IsArtifact { get { return true; } }
        public override bool ShowArtifactRarity { get { return false; } }
        public override int ArtifactRarity { get { return 8; } }

        public override double DefaultWeight { get { return 10.0; } }
        public override string DefaultName { get { return "A Painting From The Personal Collection Of The King"; } }

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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class KingsPainting2 : BaseDecorationArtifact
    {
        public override bool IsArtifact { get { return true; } }
        public override bool ShowArtifactRarity { get { return false; } }
        public override int ArtifactRarity { get { return 8; } }

        public override double DefaultWeight { get { return 10.0; } }
        public override string DefaultName { get { return "A Painting From The Personal Collection Of The King"; } }

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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
