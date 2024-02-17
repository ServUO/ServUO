namespace Server.Items
{
    [Flipable(0x422A, 0x422C)]
    public class GargishNewbieShield : BaseShield
    {
        [Constructable]
        public GargishNewbieShield()
            : base(0x422A)
        {
            Weight = 2.0;
	        LootType = LootType.Blessed;
	        Attributes.LowerRegCost = 16;
	        WeaponAttributes.SelfRepair = 5;
        }

        public GargishNewbieShield(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 8;
        public override int BaseFireResistance => 8;
        public override int BaseColdResistance => 8;
        public override int BasePoisonResistance => 6;
        public override int BaseEnergyResistance => 8;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 300;
        // public override int StrReq => 20;

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
        }
    }
}
