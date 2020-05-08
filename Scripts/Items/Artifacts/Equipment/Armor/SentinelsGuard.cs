namespace Server.Items
{
    public class SentinelsGuard : OrderShield
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1079792;// Sentinel's Guard
        public override int ArtifactRarity => 11;
        public override int BasePhysicalResistance => 16;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;

        [Constructable]
        public SentinelsGuard()
            : base()
        {
            Hue = 0x21;
        }

        public SentinelsGuard(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
