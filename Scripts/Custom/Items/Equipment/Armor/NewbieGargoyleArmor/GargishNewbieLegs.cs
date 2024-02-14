namespace Server.Items
{
    public class GargishNewbieLegs : BaseArmor
    {
        [Constructable]
        public GargishNewbieLegs()
            : this(0)
        {
        }

        [Constructable]
        public GargishNewbieLegs(int hue)
            : base(0x305)
        {
            Weight = 5.0;
            Hue = hue;
	        LootType = LootType.Blessed;
	        Attributes.LowerRegCost = 17;
	        WeaponAttributes.SelfRepair = 5;
	        ArmorAttributes.LowerStatReq = 100;
        }

        public GargishNewbieLegs(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 7;
        public override int BaseFireResistance => 7;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 7;
        public override int BaseEnergyResistance => 6;

        public override int InitMinHits => 30;
        public override int InitMaxHits => 50;

        public override int StrReq => 20;

        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
