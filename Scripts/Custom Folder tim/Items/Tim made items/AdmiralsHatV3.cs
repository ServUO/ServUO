using System;

namespace Server.Items
{
    public class AdmiralsHat : BaseArmor
	{
		
        [Constructable]
        public AdmiralsHat() : base (0x171B)
        {
			this.Name = "Admiral's Hat";
			this.Hue = 2729;
			this.Weight = 3;

			this.SkillBonuses.SetValues(0, SkillName.Swords, 20.0);
			this.SkillBonuses.SetValues(1, SkillName.Anatomy, 10);
			this.SkillBonuses.SetValues(2, SkillName.Tactics, 10);

			this.Attributes.BonusDex = 8;
			this.Attributes.BonusInt = 15;
			this.Attributes.AttackChance = 15;
			this.Attributes.NightSight = 1;
			this.Attributes.LowerRegCost = 20;
			this.Attributes.NightSight = 1;

			
		}

        public AdmiralsHat(Serial serial)
            : base(serial)
        {
        }
		
		public override ArmorMaterialType MaterialType
        {
            get
            {
                return ArmorMaterialType.Cloth;
            }
        }

        public override int LabelNumber
        {
            get
            {
                return 1094911;
            }
        }// Captain John's Hat [Replica]
        public override int BasePhysicalResistance
        {
            get
            {
                return 4;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 6;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 9;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 7;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 8;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
            }
        }
        public override bool CanFortify
        {
            get
            {
                return false;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}