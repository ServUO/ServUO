namespace Server.Items
{
    public class DragonJadeEarrings : GargishEarrings
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113720;  // Dragon Jade Earrings

        public override int BasePhysicalResistance => 9;
        public override int BaseFireResistance => 16;
        public override int BaseColdResistance => 5;
        public override int BasePoisonResistance => 13;
        public override int BaseEnergyResistance => 3;

        [Constructable]
        public DragonJadeEarrings()
        {
            Hue = 2129;
            Attributes.BonusDex = 5;
            Attributes.BonusStr = 5;
            Attributes.RegenHits = 2;
            Attributes.RegenStam = 3;
            Attributes.LowerManaCost = 5;
            AbsorptionAttributes.EaterFire = 10;
        }

        public DragonJadeEarrings(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

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
