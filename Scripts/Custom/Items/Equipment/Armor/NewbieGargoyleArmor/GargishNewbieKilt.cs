namespace Server.Items
{
    public class GargishNewbieKilt : BaseArmor
    {
        [Constructable]
        public GargishNewbieKilt()
            : this(0)
        {
        }

        [Constructable]
        public GargishNewbieKilt(int hue)
            : base(0x311)
        {
            Layer = Layer.Gloves;
            Weight = 5.0;
            Hue = hue;
            LootType = LootType.Blessed;
            Attributes.LowerRegCost = 17;
            WeaponAttributes.SelfRepair = 5;
            ArmorAttributes.LowerStatReq = 100;
        }

        public GargishNewbieKilt(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 8;
        public override int BaseFireResistance => 8;
        public override int BaseColdResistance => 8;
        public override int BasePoisonResistance => 7;
        public override int BaseEnergyResistance => 8;

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