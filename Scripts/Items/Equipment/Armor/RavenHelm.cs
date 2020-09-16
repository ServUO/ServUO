namespace Server.Items
{
    [Flipable(0x2B71, 0x3168)]
    public class RavenHelm : BaseArmor
    {
        [Constructable]
        public RavenHelm()
            : base(0x2B71)
        {
            Weight = 5.0;
        }

        public RavenHelm(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 1;
        public override int BaseColdResistance => 2;
        public override int BasePoisonResistance => 2;
        public override int BaseEnergyResistance => 5;
        public override int InitMinHits => 50;
        public override int InitMaxHits => 65;
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
