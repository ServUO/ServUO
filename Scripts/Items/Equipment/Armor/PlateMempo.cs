namespace Server.Items
{
    public class PlateMempo : BaseArmor
    {
        [Constructable]
        public PlateMempo()
            : base(0x2779)
        {
            Weight = 3.0;
        }

        public PlateMempo(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 2;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 2;
        public override int InitMinHits => 60;
        public override int InitMaxHits => 70;
        public override int StrReq => 50;
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Plate;
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
