namespace Server.Items
{
    public class ElegantCollar : BaseArmor
    {
        public override int LabelNumber => 1159224;  // elegant collar

        public override int BasePhysicalResistance => 2;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 3;
        public override int InitMinHits => 35;
        public override int InitMaxHits => 50;
        public override int StrReq => 30;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;


        [Constructable]
        public ElegantCollar()
            : base(0xA40F)
        {
            Layer = Layer.Neck;
            Weight = 3.0;
        }

        public ElegantCollar(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }
    }
}
