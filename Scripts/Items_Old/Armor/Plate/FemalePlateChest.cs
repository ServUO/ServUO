using System;

namespace Server.Items
{
    [FlipableAttribute(0x1c04, 0x1c05)]
    public class FemalePlateChest : BaseArmor
    {
        [Constructable]
        public FemalePlateChest()
            : base(0x1C04)
        {
            this.Weight = 4.0;
        }

        public FemalePlateChest(Serial serial)
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
                return 50;
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
                return 95;
            }
        }
        public override int OldStrReq
        {
            get
            {
                return 45;
            }
        }
        public override int OldDexBonus
        {
            get
            {
                return -5;
            }
        }
        public override bool AllowMaleWearer
        {
            get
            {
                return false;
            }
        }
        public override int ArmorBase
        {
            get
            {
                return 30;
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

            if (this.Weight == 1.0)
                this.Weight = 4.0;
        }
    }
}