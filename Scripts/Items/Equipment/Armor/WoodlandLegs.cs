namespace Server.Items
{
    [Flipable(0x2B6B, 0x3162)]
    public class WoodlandLegs : BaseArmor
    {
        [Constructable]
        public WoodlandLegs()
            : base(0x2B6B)
        {
            Weight = 8.0;
        }

        public WoodlandLegs(Serial serial)
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
        public override int StrReq => 90;
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
