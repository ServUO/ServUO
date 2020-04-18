using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTailoring), typeof(GargishShadowCloakOfRejuvenation))]
    public class ShadowCloakOfRejuvenation : Cloak
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1115649;  // Shadow Cloak Of Rejuvenation

        [Constructable]
        public ShadowCloakOfRejuvenation()
        {
            Hue = 1884;
            Attributes.RegenMana = 1;
            Attributes.RegenHits = 1;
            Attributes.RegenStam = 1;
            Attributes.LowerManaCost = 2;
        }

        public ShadowCloakOfRejuvenation(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class GargishShadowCloakOfRejuvenation : GargishClothWingArmor
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1115649;  // Shadow Cloak Of Rejuvenation

        [Constructable]
        public GargishShadowCloakOfRejuvenation()
        {
            Hue = 1884;
            Attributes.RegenMana = 1;
            Attributes.RegenHits = 1;
            Attributes.RegenStam = 1;
            Attributes.LowerManaCost = 2;
        }

        public GargishShadowCloakOfRejuvenation(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}