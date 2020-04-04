using Server.Engines.Craft;
using System;

namespace Server.Items
{
    public class GargishPlateArms : BaseArmor
    {
        [Constructable]
        public GargishPlateArms()
            : this(0)
        {
        }

        [Constructable]
        public GargishPlateArms(int hue)
            : base(0x308)
        {
            Weight = 5.0;
            Hue = hue;
        }

        public GargishPlateArms(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance { get { return 8; } }
        public override int BaseFireResistance { get { return 6; } }
        public override int BaseColdResistance { get { return 5; } }
        public override int BasePoisonResistance { get { return 6; } }
        public override int BaseEnergyResistance { get { return 5; } }

        public override int InitMinHits { get { return 50; } }
        public override int InitMaxHits { get { return 65; } }

        public override int StrReq { get { return 80; } }
        
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
