using System;

namespace Server.Items
{
    public class AssassinChest : LeatherChest
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public AssassinChest()
            : base()
        {
            this.SetHue = 0x455;
			
            this.Attributes.BonusStam = 2;
            this.Attributes.WeaponSpeed = 5;		
			
            this.SetSkillBonuses.SetValues(0, SkillName.Stealth, 30);
			
            this.SetSelfRepair = 3;
			
            this.SetAttributes.BonusDex = 12;
			
            this.SetPhysicalBonus = 5;
            this.SetFireBonus = 4;
            this.SetColdBonus = 3;
            this.SetPoisonBonus = 4;
            this.SetEnergyBonus = 4;
        }

        public AssassinChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074304;
            }
        }// Assassin Armor
        public override SetItem SetID
        {
            get
            {
                return SetItem.Assassin;
            }
        }
        public override int Pieces
        {
            get
            {
                return 4;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 9;
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
                return 3;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 8;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 4;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }
}