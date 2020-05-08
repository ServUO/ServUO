namespace Server.Items
{
    public class ArcaneShield : WoodenKiteShield
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1061101;// Arcane Shield 
        public override int ArtifactRarity => 11;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public ArcaneShield()
        {
            Hue = 0x556;
            Attributes.NightSight = 1;
            Attributes.SpellChanneling = 1;
            Attributes.DefendChance = 15;
            Attributes.CastSpeed = 1;
        }

        public ArcaneShield(Serial serial)
            : base(serial)
        {
        }

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
