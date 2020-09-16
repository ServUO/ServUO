namespace Server.Items
{
    [Flipable(0x2643, 0x2644)]
    public class DragonGloves : BaseArmor
    {
        [Constructable]
        public DragonGloves()
            : base(0x2643)
        {
            Weight = 2.0;
        }

        public DragonGloves(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 3;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 3;
        public override int InitMinHits => 55;
        public override int InitMaxHits => 75;
        public override int StrReq => 75;
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Dragon;
        public override CraftResource DefaultResource => CraftResource.RedScales;
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
