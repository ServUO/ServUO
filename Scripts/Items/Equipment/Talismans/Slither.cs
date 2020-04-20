namespace Server.Items
{
    public class Slither : BaseTalisman
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1114782; // Slither

        [Constructable]
        public Slither()
            : base(0x2F5B)
        {
            Hue = 0x587;
            Blessed = GetRandomBlessed();
            Attributes.BonusHits = 10;
            Attributes.RegenHits = 2;
            Attributes.DefendChance = 10;
        }

        public Slither(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
        }
    }
}
