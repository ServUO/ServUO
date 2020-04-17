namespace Server.Items
{
    public class DjinnisRing : SilverRing
    {
        public override bool IsArtifact => true;
        [Constructable]
        public DjinnisRing()
        {
            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 10;
            Attributes.CastSpeed = 2;
        }

        public DjinnisRing(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1094927;// Djinni's Ring [Replica]
        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;
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