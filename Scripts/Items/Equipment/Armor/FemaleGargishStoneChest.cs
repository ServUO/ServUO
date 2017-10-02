using System;

namespace Server.Items
{
    public class FemaleGargishStoneChest : BaseArmor
    {
        [Constructable]
        public FemaleGargishStoneChest()
            : this(0)
        {
        }

        [Constructable]
        public FemaleGargishStoneChest(int hue)
            : base(0x285)
        {
            Weight = 15.0;
            Hue = hue;
        }

        public FemaleGargishStoneChest(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance { get { return 6; } }
        public override int BaseFireResistance { get { return 6; } }
        public override int BaseColdResistance { get { return 4; } }
        public override int BasePoisonResistance { get { return 8; } }
        public override int BaseEnergyResistance { get { return 6; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 50; } }

        public override int AosStrReq { get { return 40; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

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