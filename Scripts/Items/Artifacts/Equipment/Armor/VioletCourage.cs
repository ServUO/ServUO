namespace Server.Items
{
    public class VioletCourage : FemalePlateChest
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1063471; // Violet Courage
        public override int BasePhysicalResistance => 14;
        public override int BaseFireResistance => 12;
        public override int BaseColdResistance => 12;
        public override int BasePoisonResistance => 8;
        public override int BaseEnergyResistance => 9;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public VioletCourage()
        {
            Hue = Utility.RandomBool() ? 0x486 : 0x490;
            Attributes.Luck = 95;
            Attributes.DefendChance = 15;
            ArmorAttributes.LowerStatReq = 100;
            ArmorAttributes.MageArmor = 1;
        }

        public VioletCourage(Serial serial)
            : base(serial)
        {
        }

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
