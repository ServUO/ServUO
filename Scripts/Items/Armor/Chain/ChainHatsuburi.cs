using System;

namespace Server.Items
{
    public class ChainHatsuburi : BaseArmor
    {
        [Constructable]
        public ChainHatsuburi()
            : base(0x2774)
        {
            this.Weight = 7.0;
        }

        public ChainHatsuburi(Serial serial)
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
                return 2;
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
                return 2;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 4;
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
                return 75;
            }
        }
        public override int AosStrReq
        {
            get
            {
                return 50;
            }
        }
        public override int OldStrReq
        {
            get
            {
                return 50;
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
                return ArmorMaterialType.Chainmail;
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