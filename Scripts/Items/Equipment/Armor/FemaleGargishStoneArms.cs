using System;

namespace Server.Items
{
    public class FemaleGargishStoneArms : BaseArmor
    {
        [Constructable]
        public FemaleGargishStoneArms()
            : this(0)
        {
        }

        [Constructable]
        public FemaleGargishStoneArms(int hue)
            : base(0x283)
        {
            Weight = 10.0;
            Hue = hue;
        }

        public FemaleGargishStoneArms(Serial serial)
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

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Stone; } }

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