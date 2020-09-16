namespace Server.Items
{
    [Flipable(0x2B6C, 0x3163)]
    public class WoodlandArms : BaseArmor
    {
        [Constructable]
        public WoodlandArms()
            : base(0x2B6C)
        {
            Weight = 5.0;
        }

        public WoodlandArms(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 2;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 2;
        public override int InitMinHits => 50;
        public override int InitMaxHits => 65;
        public override int StrReq => 80;
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Wood;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
