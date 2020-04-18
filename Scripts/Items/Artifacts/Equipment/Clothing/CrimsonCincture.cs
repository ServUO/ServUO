using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTailoring), typeof(GargishCrimsonCincture))]
    public class CrimsonCincture : HalfApron
    {
        public override bool IsArtifact => true;
        [Constructable]
        public CrimsonCincture()
            : base()
        {
            Hue = 0x485;
            Attributes.BonusDex = 5;
            Attributes.BonusHits = 10;
            Attributes.RegenHits = 2;
        }

        public CrimsonCincture(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075043;// Crimson Cincture
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

    public class GargishCrimsonCincture : GargoyleHalfApron
    {
        public override bool IsArtifact => true;

        [Constructable]
        public GargishCrimsonCincture()
            : base()
        {
            Hue = 0x485;
            Attributes.BonusDex = 5;
            Attributes.BonusHits = 10;
            Attributes.RegenHits = 2;
        }

        public GargishCrimsonCincture(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075043;// Crimson Cincture
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