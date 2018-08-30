using System;

namespace Server.Items
{
    public class BeltOfLostSouls : BaseArmor
	{
		
        [Constructable]
        public BeltOfLostSouls() : base (10128)
        {
			this.Name = "Belt of Lost Souls";
			this.Hue = 0;
			this.Weight = 1;

			this.Attributes.BonusStr = 12;
			this.Attributes.BonusDex = 12;
			this.Attributes.BonusInt = 12;
			this.Attributes.WeaponDamage = 8;
			this.Attributes.DefendChance = 12;
			this.Attributes.CastRecovery = 3;
			this.Attributes.CastSpeed = 2;
			this.Attributes.BonusHits = 4;
			this.Attributes.LowerManaCost = 5;
			this.Attributes.LowerRegCost = 20;
			this.Attributes.BonusMana = 8;
			this.Attributes.RegenMana = 4;
			this.Attributes.RegenStam = 4;
			this.Attributes.RegenHits = 4;
			this.Attributes.WeaponSpeed = 5;
	

		}

		public BeltOfLostSouls(Serial serial)
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
        }// Replica of Belt of Tokuno
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