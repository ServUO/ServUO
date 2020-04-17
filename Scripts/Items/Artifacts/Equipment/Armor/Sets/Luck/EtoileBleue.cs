namespace Server.Items
{
    public class EtoileBleue : GoldRing
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1080238;  // Etoile Bleue
        public override SetItem SetID => SetItem.Luck;
        public override int Pieces => 2;

        [Constructable]
        public EtoileBleue() : base()
        {
            this.Weight = 1.0;
            this.Hue = 1165;

            this.Attributes.Luck = 150;
            this.Attributes.CastSpeed = 1;
            this.Attributes.CastRecovery = 1;

            this.SetHue = 1165;
            this.SetAttributes.Luck = 100;
            this.SetAttributes.RegenHits = 2;
            this.SetAttributes.RegenMana = 2;
            this.SetAttributes.CastSpeed = 1;
            this.SetAttributes.CastRecovery = 4;
        }

        public EtoileBleue(Serial serial) : base(serial)
        {
        }

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
