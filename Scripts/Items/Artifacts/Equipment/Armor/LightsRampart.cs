using System;

namespace Server.Items
{
    public class LightsRampart : MetalShield, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public LightsRampart()
        {
            this.ItemID = 0x1B7B;
            this.Hue = 1272;
			
            this.Attributes.SpellChanneling = 1;
            this.Attributes.DefendChance = 20;
        }

        public LightsRampart(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112407;
            }
        }// Light's Rampart 
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
                return 4;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 13;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 3;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 3;
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