using System;

namespace Server.Items
{
    public class GargishPlateLegs : BaseArmor
    {
        [Constructable]
        public GargishPlateLegs()
            : base(0x030E)
        {
            this.Weight = 1.0;
        }

        public GargishPlateLegs(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 2;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 4;
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
                return 4;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 35;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 45;
            }
        }
        public override int AosStrReq
        {
            get
            {
                return 25;
            }
        }
        public override int OldStrReq
        {
            get
            {
                return 25;
            }
        }
        public override int ArmorBase
        {
            get
            {
                return 16;
            }
        }
        public override ArmorMaterialType MaterialType
        {
            get
            {
                return ArmorMaterialType.Plate;
            }
        }
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
        }
        public override void OnAdded(object parent)
        {
            if (parent is Mobile)
            {
                if (((Mobile)parent).Female && this.ItemID != 0x030D)
                    this.ItemID = 0x030D;
                else if (this.ItemID != 0x030E)
                    this.ItemID = 0x030E;
            }

            base.OnAdded(parent);
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