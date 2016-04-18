using System;

namespace Server.Items
{
    [FlipableAttribute(0x2B10, 0x2B11)]
    public class SpiritualityHelm : BaseArmor
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public SpiritualityHelm()
            : base(0x2B10)
        { 
            this.LootType = LootType.Blessed;
            this.Weight = 6.0;
            this.SetHue = 0;
            this.Hue = 0x226;
			
            this.SetSelfRepair = 5;
			
            this.SetPhysicalBonus = 5;
            this.SetFireBonus = 5;
            this.SetColdBonus = 5;
            this.SetPoisonBonus = 5;
            this.SetEnergyBonus = 5;
        }

        public SpiritualityHelm(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075237;
            }
        }// Helm of Spirituality (Virtue Armor Set)
        public override SetItem SetID
        {
            get
            {
                return SetItem.Virtue;
            }
        }
        public override int Pieces
        {
            get
            {
                return 8;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 8;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 8;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 7;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 9;
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
        public override int AosStrReq
        {
            get
            {
                return 25;
            }
        }
        public override ArmorMaterialType MaterialType
        {
            get
            {
                return ArmorMaterialType.Plate;
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