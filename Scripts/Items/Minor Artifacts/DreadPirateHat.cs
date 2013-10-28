using System;

namespace Server.Items
{
    public class DreadPirateHat : TricorneHat
    {
        [Constructable]
        public DreadPirateHat()
        {
            this.Hue = 0x497;

            this.SkillBonuses.SetValues(0, Utility.RandomCombatSkill(), 10.0);

            this.Attributes.BonusDex = 8;
            this.Attributes.AttackChance = 10;
            this.Attributes.NightSight = 1;
        }

        public DreadPirateHat(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063467;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 14;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 10;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 3)
            {
                this.Resistances.Cold = 0;
                this.Resistances.Poison = 0;
            }

            if (version < 1)
            {
                this.Attributes.Luck = 0;
                this.Attributes.AttackChance = 10;
                this.Attributes.NightSight = 1;
                this.SkillBonuses.SetValues(0, Utility.RandomCombatSkill(), 10.0);
                this.SkillBonuses.SetBonus(1, 0);
            }
        }
    }
}