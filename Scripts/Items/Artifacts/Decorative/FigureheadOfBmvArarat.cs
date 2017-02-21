using System;
using Server.Items;

namespace Server.Items
{
    public class FigureheadOfBmvArarat : BaseDecorationArtifact
    {
        public override int ArtifactRarity { get { return 8; } }
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public FigureheadOfBmvArarat()
            : base(0x2D0E)
        {
            this.Name = "Figurehead Of The Bmv Ararat";
            this.Weight = 10.0;
            this.Hue = 2968; // checked
        }

        public FigureheadOfBmvArarat(Serial serial)
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