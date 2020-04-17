namespace Server.Items
{
    public class HeartOfTheLion : PlateChest
    {
        public override bool IsArtifact => true;
        [Constructable]
        public HeartOfTheLion()
        {
            Hue = 0x501;
            Attributes.Luck = 95;
            Attributes.DefendChance = 15;
            ArmorAttributes.LowerStatReq = 100;
            ArmorAttributes.MageArmor = 1;
        }

        public HeartOfTheLion(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1070817;// Heart of the Lion
        public override int BasePhysicalResistance => 15;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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