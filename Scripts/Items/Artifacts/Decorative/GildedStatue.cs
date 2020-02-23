using System;

namespace Server.Items
{
    public class KingsGildedStatue : BaseDecorationArtifact
    {
        public override bool IsArtifact { get { return true; } }
        public override bool ShowArtifactRarity { get { return false; } }
        public override int ArtifactRarity { get { return 8; } }

        public override double DefaultWeight { get { return 10.0; } }
        public override string DefaultName { get { return "A Gilded Statue from the Personal Collection of the King"; } }

        [Constructable]
        public KingsGildedStatue()
            : base(0x139D)
        {
            Hue = 2721;
        }

        public KingsGildedStatue(Serial serial)
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
