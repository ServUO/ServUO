namespace Server.Items
{
    public class KrampusMinionEarrings : BaseArmor
    {
        public override int LabelNumber => 1125645;  // krampus minion earrings        

        [Constructable]
        public KrampusMinionEarrings()
            : base(0xA295)
        {
            Layer = Layer.Earrings;
        }

        public KrampusMinionEarrings(Serial serial)
            : base(serial)
        {
        }

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Chainmail;
        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        public override int BasePhysicalResistance => 1;
        public override int BaseFireResistance => 2;
        public override int BaseColdResistance => 2;
        public override int BasePoisonResistance => 2;
        public override int BaseEnergyResistance => 3;

        public override int InitMinHits => 30;
        public override int InitMaxHits => 50;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
