using System;
using Server;

namespace Server.Items
{
    public class AcademicBooksArtifact : BaseDecorationArtifact
    {
        public override int ArtifactRarity { get { return 8; } }
        public override int LabelNumber { get { return 1071202; } } // academic books


        [Constructable]
        public AcademicBooksArtifact()
            : base(0x1E25)
        {
            Hue = 2413;
        }

        public AcademicBooksArtifact(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}