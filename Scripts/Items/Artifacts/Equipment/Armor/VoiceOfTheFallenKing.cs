namespace Server.Items
{
    public class VoiceOfTheFallenKing : LeatherGorget
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1061094;// Voice of the Fallen King
        public override int ArtifactRarity => 11;
        public override int BaseColdResistance => 18;
        public override int BaseEnergyResistance => 18;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public VoiceOfTheFallenKing()
        {
            Hue = 0x76D;
            Attributes.BonusStr = 8;
            Attributes.RegenHits = 5;
            Attributes.RegenStam = 3;
        }

        public VoiceOfTheFallenKing(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
