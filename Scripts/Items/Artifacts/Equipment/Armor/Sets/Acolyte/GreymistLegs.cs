using System;

namespace Server.Items
{
    public class GreymistLegs : LeatherLegs
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public GreymistLegs()
            : base()
        {
            this.SetHue = 0xCB;		
			
            this.Attributes.BonusMana = 2;
            this.Attributes.SpellDamage = 2;
			
            this.SetAttributes.Luck = 100;
            this.SetAttributes.NightSight = 1;
			
            this.SetSelfRepair = 3;
			
            this.SetPhysicalBonus = 3;
            this.SetFireBonus = 3;
            this.SetColdBonus = 3;
            this.SetPoisonBonus = 3;
            this.SetEnergyBonus = 3;
        }

        public GreymistLegs(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074307;
            }
        }// Greymist Armor
        public override SetItem SetID
        {
            get
            {
                return SetItem.Acolyte;
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
                return 7;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 7;
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
                return 4;
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