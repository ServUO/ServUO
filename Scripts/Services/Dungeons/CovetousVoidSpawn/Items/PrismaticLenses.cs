using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTinkering), typeof(GargishPrismaticLenses))]
    public class PrismaticLenses : Glasses
    {
        public override int LabelNumber => 1152716;  // Prismatic Lenses
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public override int BasePhysicalResistance => 18;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 17;
        public override int BaseEnergyResistance => 6;

        public override bool IsArtifact => true;

        [Constructable]
        public PrismaticLenses()
        {
            Hue = 2068;
            WeaponAttributes.HitLowerDefend = 30;
            Attributes.RegenHits = 2;
            Attributes.RegenStam = 3;
            Attributes.WeaponDamage = 25;
        }

        public PrismaticLenses(Serial serial)
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

    public class GargishPrismaticLenses : GargishGlasses
    {
        public override int LabelNumber => 1152716;  // Prismatic Lenses

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public override int BasePhysicalResistance => 18;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 17;
        public override int BaseEnergyResistance => 6;

        public override bool IsArtifact => true;

        [Constructable]
        public GargishPrismaticLenses()
        {
            Hue = 2068;
            WeaponAttributes.HitLowerDefend = 30;
            Attributes.RegenHits = 2;
            Attributes.RegenStam = 3;
            Attributes.WeaponDamage = 25;
        }

        public GargishPrismaticLenses(Serial serial)
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