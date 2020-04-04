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
            LootType = LootType.Blessed;
            Weight = 7.0;
            SetHue = 0;
            Hue = 0x226;
			
            SetSelfRepair = 5;
			
            SetPhysicalBonus = 5;
            SetFireBonus = 5;
            SetColdBonus = 5;
            SetPoisonBonus = 5;
            SetEnergyBonus = 5;
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
        public override int StrReq
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