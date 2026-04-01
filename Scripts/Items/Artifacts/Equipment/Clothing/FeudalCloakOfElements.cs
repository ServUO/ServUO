using System;

namespace Server.Items
{
    public class FeudalCloakOfElements : Cloak
    {
        public override bool IsArtifact { get { return true; } }

        public static SkillName GetRandomSkill()
        {
            SkillName[] skills = new SkillName[] { SkillName.Magery, SkillName.Mysticism, SkillName.Necromancy, SkillName.Spellweaving, SkillName.Chivalry };
            return skills[Utility.Random(skills.Length)];
        }

        [Constructable]
        public FeudalCloakOfElements()
        {
            Name = "Feudal Cloak of Elements";
            Hue = 1160;

            SAAbsorptionAttributes.EaterDamage = 15;

            Resistances.Fire = 15;

            SkillBonuses.SetValues(0, GetRandomSkill(), 15.0);
            Attributes.RegenHits = 2;
            Attributes.RegenStam = 3;
            Attributes.RegenMana = 2;
            Attributes.Luck = 150;
            Attributes.CastSpeed = 1;
            Attributes.SpellDamage = 15;
        }

        public FeudalCloakOfElements(Serial serial)
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
            reader.ReadInt();
        }
    }

    public class WingArmorOfElements : GargishLeatherWingArmor
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public WingArmorOfElements()
        {
            Name = "Wing Armor of Elements";
            Hue = 1160;

            AbsorptionAttributes.EaterDamage = 15;

            SkillBonuses.SetValues(0, FeudalCloakOfElements.GetRandomSkill(), 15.0);
            Attributes.RegenHits = 2;
            Attributes.RegenStam = 3;
            Attributes.RegenMana = 2;
            Attributes.Luck = 150;
            Attributes.CastSpeed = 1;
            Attributes.SpellDamage = 15;
        }

        public WingArmorOfElements(Serial serial)
            : base(serial)
        {
        }

        public override int BaseFireResistance { get { return 15; } }
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
