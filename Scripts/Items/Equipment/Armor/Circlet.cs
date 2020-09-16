namespace Server.Items
{
    [Flipable(0x2B6E, 0x3165)]
    public class Circlet : BaseArmor
    {
        [Constructable]
        public Circlet()
            : base(0x2B6E)
        {
            Weight = 2.0;
        }

        public Circlet(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 1;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 2;
        public override int BasePoisonResistance => 2;
        public override int BaseEnergyResistance => 5;
        public override int InitMinHits => 50;
        public override int InitMaxHits => 65;
        public override int StrReq => 10;
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Plate;
        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

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
