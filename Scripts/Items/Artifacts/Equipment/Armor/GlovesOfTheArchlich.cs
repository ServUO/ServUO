using System;

namespace Server.Items
{
    public class GlovesOfTheArchlich : BoneGloves
    {
        public override bool IsArtifact { get { return true; } }

        public static SkillName GetRandomSkill()
        {
            SkillName[] skills = new SkillName[] { SkillName.Magery, SkillName.Mysticism, SkillName.Necromancy, SkillName.Chivalry, SkillName.Bushido, SkillName.Ninjitsu, SkillName.MagicResist };
            return skills[Utility.Random(skills.Length)];
        }

        [Constructable]
        public GlovesOfTheArchlich()
        {
            Name = "Gloves of the Archlich";
            Hue = 1150;

            AbsorptionAttributes.EaterFire = 15;

            SkillBonuses.SetValues(0, GetRandomSkill(), 20.0);
            Attributes.BonusStr = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusMana = 8;
            Attributes.RegenHits = 3;
            Attributes.RegenMana = 3;
            Attributes.LowerManaCost = 10;
            Attributes.LowerRegCost = 20;
            Attributes.CastSpeed = 1;
            Attributes.CastRecovery = 1;

            ArmorAttributes.MageArmor = 1;
        }

        public GlovesOfTheArchlich(Serial serial)
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
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class GargishKiltOfTheArchlich : GargishPlateKilt
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishKiltOfTheArchlich()
        {
            Name = "Gargish Kilt of the Archlich";
            Hue = 1150;

            AbsorptionAttributes.EaterFire = 15;

            SkillBonuses.SetValues(0, GlovesOfTheArchlich.GetRandomSkill(), 20.0);
            Attributes.BonusStr = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusMana = 8;
            Attributes.RegenHits = 3;
            Attributes.RegenMana = 3;
            Attributes.LowerManaCost = 10;
            Attributes.LowerRegCost = 20;
            Attributes.CastSpeed = 1;
            Attributes.CastRecovery = 1;

            ArmorAttributes.MageArmor = 1;
        }

        public GargishKiltOfTheArchlich(Serial serial)
            : base(serial)
        {
        }

        public override int AosStrReq { get { return 55; } }
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
