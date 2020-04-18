namespace Server.Items
{
    public class KnightsWarCleaver : WarCleaver
    {
        public override bool IsArtifact => true;
        [Constructable]
        public KnightsWarCleaver()
        {
            Attributes.RegenHits = 3;
        }

        public KnightsWarCleaver(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073525;// knight's war cleaver
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