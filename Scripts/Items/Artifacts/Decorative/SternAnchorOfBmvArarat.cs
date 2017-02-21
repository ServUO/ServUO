using System;
using Server.Items;

namespace Server.Items
{
    public class SternAnchorOfBmvArarat : BaseDecorationArtifact
    {
        public override int ArtifactRarity { get { return 8; } }
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public SternAnchorOfBmvArarat()
            : base(0x14F7)
        {
            this.Name = "Stern Anchor of the BMV Ararat";
            this.Weight = 10.0;
            this.Hue = 2959; // checked
        }

        public SternAnchorOfBmvArarat(Serial serial)
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