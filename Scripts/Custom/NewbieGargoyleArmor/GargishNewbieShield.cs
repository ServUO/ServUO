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
	        WeaponAttributes.SelfRepair = 5;
	        PhysicalBonus = 3;
	        FireBonus = 1;
	        ColdBonus = 2;
	        PoisonBonus = 2;
	        EnergyBonus = 2;	

        }

        public GargishNewbieShield(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 300;
        public override int StrReq => 95;

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
