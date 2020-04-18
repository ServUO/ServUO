namespace Server.Items
{
    public class DemonBridleRing : GoldRing
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113651;  // Demon Bridle Ring

        [Constructable]
        public DemonBridleRing()
        {
            Hue = 39;
            Attributes.CastRecovery = 2;
            Attributes.CastSpeed = 1;
            Attributes.RegenHits = 1;
            Attributes.RegenMana = 1;
            Attributes.DefendChance = 10;
            Attributes.LowerManaCost = 4;
            Resistances.Fire = 5;
        }

        public DemonBridleRing(Serial serial)
            : base(serial)
        {
        }

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