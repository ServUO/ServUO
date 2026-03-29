using System;

namespace Server.Items
{
    public class ShugenjasRaiment : PlateDo
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ShugenjasRaiment()
        {
            Name = "Shugenja's Raiment";
            Hue = 1266;

            Attributes.BonusInt = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusMana = 10;
            Attributes.RegenHits = 3;
            Attributes.RegenMana = 3;
            Attributes.SpellDamage = 30;
            Attributes.CastRecovery = 1;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 20;

            ArmorAttributes.MageArmor = 1;

            SkillBonuses.SetValues(0, GetRandomSkill(), 20.0);
        }

        public static SkillName GetRandomSkill()
        {
            SkillName[] skills = new SkillName[]
            {
                SkillName.MagicResist, SkillName.EvalInt, SkillName.Focus,
                SkillName.SpiritSpeak, SkillName.Anatomy
            };
            return skills[Utility.Random(skills.Length)];
        }

        public ShugenjasRaiment(Serial serial)
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

    public class GargishShugenjasRaiment : GargishPlateChest
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishShugenjasRaiment()
        {
            Name = "Gargish Shugenja's Raiment";
            Hue = 1266;

            Attributes.BonusInt = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusMana = 10;
            Attributes.RegenHits = 3;
            Attributes.RegenMana = 3;
            Attributes.SpellDamage = 30;
            Attributes.CastRecovery = 1;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 20;

            ArmorAttributes.MageArmor = 1;

            SkillBonuses.SetValues(0, ShugenjasRaiment.GetRandomSkill(), 20.0);
        }

        public GargishShugenjasRaiment(Serial serial)
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
