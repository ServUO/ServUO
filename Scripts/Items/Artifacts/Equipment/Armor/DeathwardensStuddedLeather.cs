using System;

namespace Server.Items
{
    public class DeathwardensGreaves : StuddedLegs
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public DeathwardensGreaves()
        {
            Name = "Deathwarden's Studded Leather Greaves";
            Hue = 1150;
            Weight = 4.0;

            AbsorptionAttributes.EaterKinetic = 15;

            SkillBonuses.SetValues(0, SkillName.EvalInt, 20.0);
            Attributes.BonusStr = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusMana = 12;
            Attributes.RegenMana = 4;
            Attributes.RegenHits = 4;
            Attributes.SpellDamage = 8;
            Attributes.LowerManaCost = 10;
            Attributes.LowerRegCost = 20;
            Attributes.CastRecovery = 1;
        }

        public DeathwardensGreaves(Serial serial)
            : base(serial)
        {
        }

        public override int AosStrReq { get { return 20; } }
        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 15; } }
        public override int BaseColdResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 15; } }
        public override int BaseEnergyResistance { get { return 15; } }
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

    public class GargishDeathwardensGreaves : GargishStoneLegs
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishDeathwardensGreaves()
        {
            Name = "Gargish Deathwarden's Greaves";
            Hue = 1150;
            Weight = 5.0;

            AbsorptionAttributes.EaterKinetic = 15;

            SkillBonuses.SetValues(0, SkillName.EvalInt, 20.0);
            Attributes.BonusStr = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusMana = 12;
            Attributes.RegenMana = 4;
            Attributes.RegenHits = 4;
            Attributes.SpellDamage = 8;
            Attributes.LowerManaCost = 10;
            Attributes.LowerRegCost = 20;
            Attributes.CastRecovery = 1;
        }

        public GargishDeathwardensGreaves(Serial serial)
            : base(serial)
        {
        }

        public override int AosStrReq { get { return 20; } }
        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 15; } }
        public override int BaseColdResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 15; } }
        public override int BaseEnergyResistance { get { return 15; } }
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
