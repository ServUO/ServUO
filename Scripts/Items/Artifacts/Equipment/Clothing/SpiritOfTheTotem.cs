using System;

namespace Server.Items
{
    public class SpiritOfTheTotem : BearMask
	{
		public override bool IsArtifact { get { return true; } }

        public static SkillName GetRandomSkill()
        {
            SkillName[] skills = new SkillName[] { SkillName.Swords, SkillName.Fencing, SkillName.Macing };
            return skills[Utility.Random(skills.Length)];
        }

        [Constructable]
        public SpiritOfTheTotem()
        {
            Hue = 0x455;
            SkillBonuses.SetValues(0, GetRandomSkill(), 20.0);
            Attributes.BonusStr = 20;
            Attributes.BonusDex = 20;
            Attributes.BonusInt = 20;
            Attributes.ReflectPhysical = 15;
            Attributes.AttackChance = 15;
            Attributes.WeaponDamage = 25;
        }

        public SpiritOfTheTotem(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061599;
            }
        }// Spirit of the Totem
        public override int ArtifactRarity
        {
            get
            {
                return 11;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 15;
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

            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.Resistances.Physical = 0;
                        break;
                    }
            }
        }
    }
}