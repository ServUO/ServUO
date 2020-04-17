namespace Server.Items
{
    public class HumanFeyLeggings : ChainLegs
    {
        public override bool IsArtifact => true;
        [Constructable]
        public HumanFeyLeggings()
        {
            Attributes.BonusHits = 6;
            Attributes.DefendChance = 20;
            ArmorAttributes.MageArmor = 1;
        }

        public HumanFeyLeggings(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075041;// Fey Leggings
        public override int BasePhysicalResistance => 12;
        public override int BaseFireResistance => 8;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 4;
        public override int BaseEnergyResistance => 19;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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