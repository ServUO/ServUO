using System;
using Server;

namespace Server.Items
{
    public class TigerPeltHelm : BaseArmor
    {
        public override int BasePhysicalResistance { get { return 2; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 3; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 3; } }

        public override int InitMinHits { get { return 30; } }
        public override int InitMaxHits { get { return 40; } }

        public override int StrReq { get { return 25; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Leather; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }

        public override int LabelNumber { get { return 1109632; } } // Tiger Pelt Helm

        [Constructable]
        public TigerPeltHelm()
            : base(0x7828)
        {
            Weight = 2.0;
        }

        public TigerPeltHelm(Serial serial)
            : base(serial)
        {
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
