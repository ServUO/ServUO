namespace Server.Items
{
    public class HolyKnightsBreastplate : PlateChest
    {
        public override bool IsArtifact => true;
        [Constructable]
        public HolyKnightsBreastplate()
        {
            Hue = 0x47E;
            Attributes.BonusHits = 10;
            Attributes.ReflectPhysical = 15;
        }

        public HolyKnightsBreastplate(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1061097;// Holy Knight's Breastplate
        public override int ArtifactRarity => 11;
        public override int BasePhysicalResistance => 35;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1)
                this.PhysicalBonus = 0;
        }
    }
}