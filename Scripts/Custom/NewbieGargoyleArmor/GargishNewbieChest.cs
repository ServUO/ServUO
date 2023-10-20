namespace Server.Items
{
    public class GargishNewbieChest : BaseArmor
    {
        [Constructable]
        public GargishNewbieChest()
            : this(0)
        {
        }

        [Constructable]
        public GargishNewbieChest(int hue)
            : base(0x0304)
        {
            Weight = 2.0;
            Hue = hue;
	        LootType = LootType.Blessed;
	        Attributes.LowerRegCost = 20;
	        WeaponAttributes.SelfRepair = 5;
	        ArmorAttributes.LowerStatReq = 100;
	        PhysicalBonus = 3;
	        FireBonus = 1;
	        ColdBonus = 2;
	        PoisonBonus = 2;
	        EnergyBonus = 2;
		
        }

        public GargishNewbieChest(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 15;
        public override int BaseFireResistance => 15;
        public override int BaseColdResistance => 15;
        public override int BasePoisonResistance => 15;
        public override int BaseEnergyResistance => 15;

        public override int InitMinHits => 30;
        public override int InitMaxHits => 50;

        public override int StrReq => 25;

        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
