using System;

namespace Server.Items
{
    [FlipableAttribute(0x2B0E, 0x2B0F)]
    public class HonestyGorget : BaseArmor
    {
        [Constructable]
        public HonestyGorget()
            : base(0x2B0E)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 2.0;
            this.SetHue = 0;
            this.Hue = 0x226;
			
            this.SetSelfRepair = 5;
			
            this.SetPhysicalBonus = 5;
            this.SetFireBonus = 5;
            this.SetColdBonus = 5;
            this.SetPoisonBonus = 5;
            this.SetEnergyBonus = 5;
        }

        public HonestyGorget(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075189;
            }
        }// Gorget of Honesty (Virtue Armor Set)
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
                return 9;
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
                return 7;
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
                return 45;
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