namespace Server.Items
{
    [Flipable(0x2B73, 0x316A)]
    public class WingedHelm : BaseArmor
    {
        [Constructable]
        public WingedHelm()
            : base(0x2B73)
        {
            Weight = 5.0;
        }

        public WingedHelm(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 1;
        public override int BaseColdResistance => 2;
        public override int BasePoisonResistance => 2;
        public override int BaseEnergyResistance => 5;
        public override int InitMinHits => 45;
        public override int InitMaxHits => 55;
        public override int StrReq => 25;
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Plate;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
