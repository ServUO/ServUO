using System;
using Server.Engines.Craft;

namespace Server.Items
{
    public class ElegantCollar : BaseArmor
    {
        public override int LabelNumber { get { return 1159224; } } // elegant collar

        public override int BasePhysicalResistance { get { return 2; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 3; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 3; } }
        public override int InitMinHits { get { return 35; } }
        public override int InitMaxHits { get { return 50; } }
        public override int AosStrReq { get { return 30; } }
        public override int ArmorBase { get { return 7; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Leather; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }


        [Constructable]
        public ElegantCollar()
            : base(0xA40F)
        {
            Layer = Layer.Neck;
            Weight = 3.0;
        }

        public ElegantCollar(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }
    }
}
