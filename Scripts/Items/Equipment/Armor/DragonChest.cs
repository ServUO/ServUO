using System;

namespace Server.Items
{
    [FlipableAttribute(0x2641, 0x2642)]
    public class DragonChest : BaseArmor
    {
        [Constructable]
        public DragonChest()
            : base(0x2641)
        {
            this.Weight = 10.0;
        }

        public DragonChest(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 3;
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
                return 3;
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
                return 75;
            }
        }
        public override int OldStrReq
        {
            get
            {
                return 60;
            }
        }
        public override int OldDexBonus
        {
            get
            {
                return -8;
            }
        }
        public override int ArmorBase
        {
            get
            {
                return 40;
            }
        }
        public override ArmorMaterialType MaterialType
        {
            get
            {
                return ArmorMaterialType.Dragon;
            }
        }
        public override CraftResource DefaultResource
        {
            get
            {
                return CraftResource.RedScales;
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

            if (this.Weight == 1.0)
                this.Weight = 15.0;
        }
    }
}