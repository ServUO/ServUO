using System;

namespace Server.Items
{
    public class MyrmidonGorget : StuddedGorget
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public MyrmidonGorget()
            : base()
        {
            this.SetHue = 0x331;
			
            this.Attributes.BonusStr = 1;
            this.Attributes.BonusHits = 2;
			
            this.SetAttributes.Luck = 500;
            this.SetAttributes.NightSight = 1;
			
            this.SetSelfRepair = 3;			
            this.SetPhysicalBonus = 3;
            this.SetFireBonus = 3;
            this.SetColdBonus = 3;
            this.SetPoisonBonus = 3;
            this.SetEnergyBonus = 3;
        }

        public MyrmidonGorget(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074306;
            }
        }// Myrmidon Armor
        public override SetItem SetID
        {
            get
            {
                return SetItem.Myrmidon;
            }
        }
        public override int Pieces
        {
            get
            {
                return 6;
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
                return 5;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 3;
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