using System;

namespace Server.Items
{
    public class HunterArms : LeafArms
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public HunterArms()
            : base()
        {
            this.SetHue = 0x483;
			
            this.Attributes.RegenHits = 1;
            this.Attributes.Luck = 50;
			
            this.SetAttributes.BonusDex = 10;
			
            this.SetSkillBonuses.SetValues(0, SkillName.Stealth, 40);
			
            this.SetSelfRepair = 3;
			
            this.SetPhysicalBonus = 5;
            this.SetFireBonus = 4;
            this.SetColdBonus = 3;
            this.SetPoisonBonus = 4;
            this.SetEnergyBonus = 4;
        }

        public HunterArms(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074301;
            }
        }// Hunter's Garb
        public override SetItem SetID
        {
            get
            {
                return SetItem.Hunter;
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