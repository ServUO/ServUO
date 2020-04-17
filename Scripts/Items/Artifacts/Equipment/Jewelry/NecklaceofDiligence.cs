namespace Server.Items
{
    public class NecklaceofDiligence : SilverNecklace
    {
        public override bool IsArtifact => true;
        [Constructable]
        public NecklaceofDiligence()
        {
            Hue = 221;
            Attributes.RegenMana = 1;
            Attributes.BonusInt = 5;
        }

        public NecklaceofDiligence(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113137;
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