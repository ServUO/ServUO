using System;

namespace Server.Items
{
    public class HexweaversVisage : WingedHelm
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public HexweaversVisage()
        {
            Name = "Hexweaver's Visage";
            Hue = 1266;

            AbsorptionAttributes.CastingFocus = 2;

            SkillBonuses.SetValues(0, GetRandomMageSkill(), 10.0);

            Attributes.BonusStr = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusMana = 8;
            Attributes.SpellDamage = 5;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 20;

            ArmorAttributes.MageArmor = 1;
        }

        public static SkillName GetRandomMageSkill()
        {
            SkillName[] skills = new SkillName[] { SkillName.Necromancy, SkillName.Mysticism, SkillName.Magery, SkillName.Chivalry };
            return skills[Utility.Random(skills.Length)];
        }

        public HexweaversVisage(Serial serial)
            : base(serial)
        {
        }

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

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class GargishHexweaversVisage : GargishGlasses
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishHexweaversVisage()
        {
            Name = "Gargish Hexweaver's Visage";
            Hue = 1266;

            AbsorptionAttributes.CastingFocus = 2;

            SkillBonuses.SetValues(0, HexweaversVisage.GetRandomMageSkill(), 10.0);

            Attributes.BonusStr = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusMana = 8;
            Attributes.SpellDamage = 5;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 20;

            ArmorAttributes.MageArmor = 1;
        }

        public GargishHexweaversVisage(Serial serial)
            : base(serial)
        {
        }

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

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
