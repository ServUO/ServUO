using System;

namespace Server.Items
{
    [FlipableAttribute(0x2B08, 0x2B09)]
    public class JusticeBreastplate : BaseArmor
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public JusticeBreastplate()
            : base(0x2B08)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 7.0;
            this.SetHue = 0;
            this.Hue = 0x226;
			
            this.SetSelfRepair = 5;
			
            this.SetPhysicalBonus = 5;
            this.SetFireBonus = 5;
            this.SetColdBonus = 5;
            this.SetPoisonBonus = 5;
            this.SetEnergyBonus = 5;
        }

        public JusticeBreastplate(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075190;
            }
        }// Breastplate of Justice (Virtue Armor Set)
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
                return 10;
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
                return 8;
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
                return 65;
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