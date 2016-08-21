using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishPlateLegs))]
    public class PlateHaidate : BaseArmor
    {
        [Constructable]
        public PlateHaidate()
            : base(0x278D)
        {
            this.Weight = 7.0;
        }

        public PlateHaidate(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 3;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 2;
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
                return 2;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 55;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 65;
            }
        }
        public override int AosStrReq
        {
            get
            {
                return 80;
            }
        }
        public override int OldStrReq
        {
            get
            {
                return 80;
            }
        }
        public override int ArmorBase
        {
            get
            {
                return 3;
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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}