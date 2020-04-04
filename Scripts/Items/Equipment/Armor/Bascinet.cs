using System;

namespace Server.Items
{
    public class Bascinet : BaseArmor
    {
		public override int BasePhysicalResistance { get { return 7; } }
        public override int BaseFireResistance { get { return 2; } }
        public override int BaseColdResistance { get { return 2; } }
        public override int BasePoisonResistance { get { return 2; } }
        public override int BaseEnergyResistance { get { return 2; } }
        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 50; } }
        public override int StrReq { get { return 40; } }
        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }
		
        [Constructable]
        public Bascinet()
            : base(0x140C)
        {
            Weight = 5.0;
        }

        public Bascinet(Serial serial)
            : base(serial)
        {
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
