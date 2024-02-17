namespace Server.Items
{
    public class GargishNewbieArms : BaseArmor
    {
        [Constructable]
        public GargishNewbieArms()
            : this(0)
        {
        }

        [Constructable]
        public GargishNewbieArms(int hue)
            : base(0x302)
        {
            Layer = Layer.Arms;
            Weight = 2.0;
            Hue = hue;
            LootType = LootType.Blessed;
	        Attributes.LowerRegCost = 17;
	        WeaponAttributes.SelfRepair = 5;
	        ArmorAttributes.LowerStatReq = 100;
		}

        public GargishNewbieArms(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 8;
        public override int BaseFireResistance => 9;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 6;
        public override int BaseEnergyResistance => 7;

        public override int InitMinHits => 30;
        public override int InitMaxHits => 50;

        // public override int StrReq => 25;

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
