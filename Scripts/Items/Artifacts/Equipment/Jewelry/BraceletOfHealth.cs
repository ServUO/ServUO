namespace Server.Items
{
    public class BraceletOfHealth : GoldBracelet
    {
        public override bool IsArtifact => true;
        [Constructable]
        public BraceletOfHealth()
        {
            Hue = 0x21;
            Attributes.BonusHits = 5;
            Attributes.RegenHits = 10;
        }

        public BraceletOfHealth(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1061103;// Bracelet of Health
        public override int ArtifactRarity => 11;
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