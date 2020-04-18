using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTinkering), typeof(GargishBrightsightLenses))]
    public class BrightsightLenses : ElvenGlasses
    {
        public override bool IsArtifact => true;
        [Constructable]
        public BrightsightLenses()
        {
            Hue = 0x501;
            Attributes.NightSight = 1;
            Attributes.RegenMana = 3;
            ArmorAttributes.SelfRepair = 3;
        }

        public BrightsightLenses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075039;// Brightsight Lenses
        public override int BasePhysicalResistance => 9;
        public override int BaseFireResistance => 29;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 8;
        public override int BaseEnergyResistance => 7;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1)
            {
                WeaponAttributes.SelfRepair = 0;
                ArmorAttributes.SelfRepair = 3;
            }
        }
    }

    [Alterable(typeof(DefTinkering), typeof(GargishBrightsightLenses))]
    public class GargishBrightsightLenses : GargishGlasses
    {
        public override bool IsArtifact => true;

        [Constructable]
        public GargishBrightsightLenses()
        {
            Hue = 0x501;
            Attributes.NightSight = 1;
            Attributes.RegenMana = 3;
            ArmorAttributes.SelfRepair = 3;
        }

        public GargishBrightsightLenses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075039;// Brightsight Lenses
        public override int BasePhysicalResistance => 9;
        public override int BaseFireResistance => 29;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 8;
        public override int BaseEnergyResistance => 7;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}