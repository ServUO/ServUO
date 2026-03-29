using System;

namespace Server.Items
{
    public class AzaroksLegplates : PlateLegs
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public AzaroksLegplates()
        {
            Name = "Azarok's Legplates";
            Hue = 1161;
            Weight = 7.0;


            Attributes.BonusDex = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusHits = 4;
            Attributes.BonusStam = 8;
            Attributes.BonusMana = 8;
            Attributes.LowerManaCost = 8;
            Attributes.WeaponDamage = 20;
            SkillBonuses.SetValues(0, SkillName.Healing, 15.0);
        }

        public AzaroksLegplates(Serial serial)
            : base(serial)
        {
        }

        public override int AosStrReq { get { return 90; } }
        public override int BasePhysicalResistance { get { return 20; } }
        public override int BaseFireResistance { get { return 20; } }
        public override int BaseColdResistance { get { return 20; } }
        public override int BasePoisonResistance { get { return 20; } }
        public override int BaseEnergyResistance { get { return 20; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class GargishAzaroksLegplates : GargishPlateLegs
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishAzaroksLegplates()
        {
            Name = "Gargish Azarok's Legplates";
            Hue = 1161;
            Weight = 7.0;


            Attributes.BonusDex = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusHits = 4;
            Attributes.BonusStam = 8;
            Attributes.BonusMana = 8;
            Attributes.LowerManaCost = 8;
            Attributes.WeaponDamage = 20;
            SkillBonuses.SetValues(0, SkillName.Healing, 15.0);
        }

        public GargishAzaroksLegplates(Serial serial)
            : base(serial)
        {
        }

        public override int AosStrReq { get { return 90; } }
        public override int BasePhysicalResistance { get { return 20; } }
        public override int BaseFireResistance { get { return 20; } }
        public override int BaseColdResistance { get { return 20; } }
        public override int BasePoisonResistance { get { return 20; } }
        public override int BaseEnergyResistance { get { return 20; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
