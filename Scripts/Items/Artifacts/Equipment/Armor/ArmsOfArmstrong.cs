namespace Server.Items
{
    public class ArmsOfArmstrong : LeatherArms
    {
        public override bool IsArtifact => true;
        [Constructable]
        public ArmsOfArmstrong()
        {
            LootType = LootType.Blessed;
            Attributes.BonusStr = 3;
            Attributes.RegenHits = 1;
        }

        public ArmsOfArmstrong(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1077675;// Arms of Armstrong
        public override int BasePhysicalResistance => 6;
        public override int BaseFireResistance => 6;
        public override int BaseColdResistance => 5;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;
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